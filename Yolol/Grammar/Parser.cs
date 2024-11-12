using System;
using Pegasus.Common;
using Yolol.Grammar.AST;

namespace Yolol.Grammar
{
    public static class Parser
    {
        public static Result<Program, ParseError> ParseProgram(string program)
        {
            try
            {
                var parser = new YololParser();
                var parsed = parser.Parse(program);
                parsed.SourceCode = program;

                return new Result<Program, ParseError>(parsed);
            }
            catch (FormatException e)
            {
                var c = e.Data["cursor"] as Cursor;
                return new Result<Program, ParseError>(new ParseError(c, e.Message));
            }
        }

        public readonly struct Result<TOk, TErr>
            where TOk : class
            where TErr : class
        {
            private readonly TOk? _ok;
            private readonly TErr? _err;

            public TOk Ok
            {
                get
                {
                    if (IsOk)
                        return _ok!;
                    throw new InvalidOperationException("Cannot get an ok value from an error result");
                }
            }

            public TErr Err {
                get
                {
                    if (!IsOk)
                        return _err!;
                    throw new InvalidOperationException("Cannot get an error value from ok result");
                }
            }

            public bool IsOk { get; }

            public Result(TOk ok)
            {
                IsOk = true;
                _ok = ok;
                _err = null;
            }

            public Result(TErr err)
            {
                IsOk = false;
                _ok = null;
                _err = err;
            }
        }

        public class ParseError
        {
            public Cursor? Cursor { get; }
            public string Message { get; }

            public ParseError(Cursor? cursor, string message)
            {
                message = message
                    .Replace("\r", "\\r", StringComparison.Ordinal)
                    .Replace("\n", "\\n", StringComparison.Ordinal);

                Cursor = cursor;
                Message = message;
            }

            public override string ToString()
            {
                if (Cursor == null)
                    return Message;

                var spaces = new string(' ', Math.Max(0, Cursor.Column - 2));

                return $"{Cursor.Subject}\n"
                     + $"{spaces}^ {Message} (Ln{Cursor.Line}, Col{Cursor.Column - 1})\n";
            }
        }
    }
}

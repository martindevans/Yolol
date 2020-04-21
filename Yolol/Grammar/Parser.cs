using System;
using Pegasus.Common;
using Yolol.Grammar.AST;

namespace Yolol.Grammar
{
    public class Parser
    {
        public static Result<Program, ParseError> ParseProgram(string program)
        {
            try
            {
                var p = new YololParser();
                return new Result<Program, ParseError>(p.Parse(program));
            }
            catch (FormatException e)
            {
                var c = (Cursor)e.Data["cursor"];
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
            public Cursor Cursor { get; }
            public string Message { get; }

            public ParseError(Cursor cursor, string message)
            {
                Cursor = cursor;
                Message = message;
            }

            public override string ToString()
            {
                return $"{Message} (Ln{Cursor.Line}, Ch{Cursor.Column})";
            }
        }
    }
}

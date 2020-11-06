using System;
using Parsers;
using Pegasus.Common;

namespace YololAssembler.Grammar
{
    internal class Parser
    {
        public static Yolol.Grammar.Parser.Result<AST.Program, Yolol.Grammar.Parser.ParseError> ParseProgram(string program)
        {
            try
            {
                var p = new YololAssemblerParser();
                return new Yolol.Grammar.Parser.Result<AST.Program, Yolol.Grammar.Parser.ParseError>(p.Parse(program));
            }
            catch (FormatException e)
            {
                var c = e.Data["cursor"] as Cursor;
                return new Yolol.Grammar.Parser.Result<AST.Program, Yolol.Grammar.Parser.ParseError>(new Yolol.Grammar.Parser.ParseError(c!, e.Message));
            }
        }
    }
}

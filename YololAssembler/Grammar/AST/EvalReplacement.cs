using System;
using Yolol.Execution.Extensions;
using YololAssembler.Grammar.Errors;

namespace YololAssembler.Grammar.AST
{
    internal class EvalReplacement
        : BaseDefine
    {
        protected override string FindRegex => $"eval\\((?<body>.*?)\\)";

        protected override string Replace(string part)
        {
            // Convert the floating expression into a statement assigning a variable;
            const string name = "eval";
            var stmtCode = $"{name}={part}";

            // Try to parse this tiny little program
            var parsed = Yolol.Grammar.Parser.ParseProgram(stmtCode);
            if (!parsed.IsOk)
                throw new CannotParseEval(part, parsed.Err);
            
            // Get the parsed expression back out
            var stmt = (Yolol.Grammar.AST.Statements.Assignment)parsed.Ok.Lines[0].Statements.Statements[0];
            var expr = stmt.Right;

            if (!expr.IsConstant)
                throw new EvalNotConst(expr);

            var v = expr.StaticEvaluate();

            if (v.Type == Yolol.Execution.Type.Number)
                return v.ToString();
            else
                return $"\"{v}\"";
        }
    }
}

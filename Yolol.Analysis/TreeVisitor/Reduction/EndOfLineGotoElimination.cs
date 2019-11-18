using System.Linq;
using Yolol.Execution.Extensions;
using Yolol.Grammar.AST;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.TreeVisitor.Reduction
{
    public class EndOfLineGotoElimination
        : BaseTreeVisitor
    {
        private uint _lineNumber;

        protected override Line Visit(Line line)
        {
            _lineNumber++;

            if (line.Statements.Statements.Count == 0)
                return base.Visit(line);

            var last = line.Statements.Statements.Last();
            if (!(last is Goto @goto))
                return base.Visit(line);

            if (!@goto.Destination.IsConstant)
                return base.Visit(line);

            var destination = @goto.Destination.StaticEvaluate();
            if (destination.Type != Execution.Type.Number)
                return base.Visit(line);

            var next = _lineNumber + 1;
            if (next < 1 || next > 20)
                next = 1;

            if (!destination.Equals(next))
                return base.Visit(line);

            return base.Visit(new Line(new StatementList(line.Statements.Statements.Take(line.Statements.Statements.Count - 1))));
        }
    }
}

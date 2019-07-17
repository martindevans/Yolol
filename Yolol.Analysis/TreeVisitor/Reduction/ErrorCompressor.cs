using JetBrains.Annotations;
using Yolol.Analysis.ControlFlowGraph.AST;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.TreeVisitor.Reduction
{
    public class ErrorCompressor
        : BaseTreeVisitor
    {
        protected override BaseStatement Visit(Conditional con)
        {
            var c = Visit(con.Condition);
            if (c is ErrorExpression)
                return new ErrorStatement();

            return con;
        }

        protected override BaseStatement Visit(Assignment ass)
        {
            var r = base.Visit(ass.Right);
            if (r is ErrorExpression)
                return new ErrorStatement();

            return ass;
        }

        protected override BaseStatement Visit(CompoundAssignment compAss)
        {
            var r = base.Visit(compAss.Right);
            if (r is ErrorExpression)
                return new ErrorStatement();

            return compAss;
        }

        protected override BaseStatement Visit(TypedAssignment ass)
        {
            var r = base.Visit(ass.Right);
            if (r is ErrorExpression)
                return new ErrorStatement();

            return ass;
        }
    }
}

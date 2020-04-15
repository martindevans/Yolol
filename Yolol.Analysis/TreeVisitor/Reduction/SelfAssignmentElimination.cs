using Yolol.Analysis.ControlFlowGraph.AST;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.TreeVisitor.Reduction
{
    public class SelfAssignmentElimination
        : BaseTreeVisitor
    {
        protected override BaseStatement Visit(Assignment ass)
        {
            if (ass.Left.IsExternal)
                return base.Visit(ass);

            if (!(ass.Right is Variable v))
                return base.Visit(ass);

            if (ass.Left == v.Name)
                return new EmptyStatement();

            return base.Visit(ass);
        }

        protected override BaseStatement Visit(TypedAssignment ass)
        {
            if (ass.Left.IsExternal)
                return base.Visit(ass);

            if (!(ass.Right is Variable v))
                return base.Visit(ass);

            if (ass.Left == v.Name)
                return new EmptyStatement();

            return base.Visit(ass);
        }
    }
}

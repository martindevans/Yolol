using Yolol.Analysis.ControlFlowGraph.AST;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.TreeVisitor.Reduction
{
    public class StripTypes
        : BaseTreeVisitor
    {
        protected override BaseStatement Visit(TypedAssignment ass)
        {
            return Visit(new Assignment(Visit(ass.Left), Visit(ass.Right)));
        }
    }
}

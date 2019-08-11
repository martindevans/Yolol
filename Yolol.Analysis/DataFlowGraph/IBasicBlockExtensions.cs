using JetBrains.Annotations;
using Yolol.Analysis.ControlFlowGraph;
using Yolol.Analysis.ControlFlowGraph.Extensions;

namespace Yolol.Analysis.DataFlowGraph
{
    public static class IBasicBlockExtensions
    {
        [NotNull] public static IDataFlowGraph DataFlowGraph([NotNull] this IBasicBlock block, [NotNull] ISingleStaticAssignmentTable ssa)
        {
            return new DataFlowGraph(block, ssa);
        }
    }
}

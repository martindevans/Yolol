using Yolol.Analysis.ControlFlowGraph;
using Yolol.Analysis.ControlFlowGraph.Extensions;

namespace Yolol.Analysis.DataFlowGraph
{
    public static class IBasicBlockExtensions
    {
        public static IDataFlowGraph DataFlowGraph(this IBasicBlock block, ISingleStaticAssignmentTable ssa)
        {
            return new DataFlowGraph(block, ssa);
        }
    }
}

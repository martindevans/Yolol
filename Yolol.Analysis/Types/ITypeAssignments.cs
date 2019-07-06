using Yolol.Analysis.ControlFlowGraph;

namespace Yolol.Analysis.Types
{
    public interface ITypeAssignments
    {
        Execution.Type? TypeOf(string varName);
    }
}

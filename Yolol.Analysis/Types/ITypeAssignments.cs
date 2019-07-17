using Yolol.Analysis.ControlFlowGraph;
using Yolol.Grammar;

namespace Yolol.Analysis.Types
{
    public interface ITypeAssignments
    {
        Execution.Type? TypeOf(VariableName varName);
    }
}

using Yolol.Execution;
using Yolol.Grammar;

namespace Yolol.Analysis.Types
{
    public interface ITypeAssignments
    {
        Type? TypeOf(VariableName varName);
    }

    public class NullTypeAssignments
        : ITypeAssignments
    {
        public Type? TypeOf(VariableName varName)
        {
            return null;
        }
    }
}

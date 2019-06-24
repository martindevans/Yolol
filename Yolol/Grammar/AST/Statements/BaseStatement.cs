using JetBrains.Annotations;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Statements
{
    public abstract class BaseStatement
    {
        [NotNull] public abstract ExecutionResult Evaluate([NotNull] MachineState state);

        [NotNull] public abstract override string ToString();
    }
}

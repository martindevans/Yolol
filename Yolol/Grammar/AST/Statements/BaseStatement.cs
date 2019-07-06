using JetBrains.Annotations;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Statements
{
    public abstract class BaseStatement
    {
        /// <summary>
        /// Check if this statement can cause a runtime error
        /// </summary>
        public abstract bool CanRuntimeError { get; }

        [NotNull] public abstract ExecutionResult Evaluate([NotNull] MachineState state);

        [NotNull] public abstract override string ToString();
    }
}

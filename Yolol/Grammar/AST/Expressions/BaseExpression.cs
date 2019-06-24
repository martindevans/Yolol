using JetBrains.Annotations;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions
{
    public abstract class BaseExpression
    {
        public abstract bool IsConstant { get; }

        public abstract Value Evaluate([NotNull] MachineState state);

        [NotNull] public abstract override string ToString();
    }
}

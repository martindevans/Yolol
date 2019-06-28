using JetBrains.Annotations;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions
{
    public abstract class BaseExpression
    {
        public abstract bool IsConstant { get; }

        /// <summary>
        /// Check if this expression is boolean - i.e. guaranteed to return `0` or `1`
        /// </summary>
        public abstract bool IsBoolean { get; }

        public abstract Value Evaluate([NotNull] MachineState state);

        [NotNull] public abstract override string ToString();
    }
}

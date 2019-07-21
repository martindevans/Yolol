using System;
using JetBrains.Annotations;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions
{
    public abstract class BaseExpression
        : IEquatable<BaseExpression>
    {
        /// <summary>
        /// Check if this expression evaluates to a constant value
        /// </summary>
        public abstract bool IsConstant { get; }

        /// <summary>
        /// Check if this expression is boolean - i.e. guaranteed to return `0` or `1`
        /// </summary>
        public abstract bool IsBoolean { get; }

        /// <summary>
        /// Check if this expression can cause a runtime error
        /// </summary>
        public abstract bool CanRuntimeError { get; }

        public abstract Value Evaluate([NotNull] MachineState state);

        public abstract bool Equals(BaseExpression other);

        [NotNull] public abstract override string ToString();
    }
}

using System;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Statements
{
    public abstract class BaseStatement
        : IEquatable<BaseStatement>
    {
        /// <summary>
        /// Check if this statement can cause a runtime error
        /// </summary>
        public abstract bool CanRuntimeError { get; }

        public abstract ExecutionResult Evaluate(MachineState state);

        public abstract bool Equals(BaseStatement? other);

        public override bool Equals(object? obj)
        {
            return Equals(obj as BaseStatement);
        }

        public abstract override int GetHashCode();

        public abstract override string ToString();
    }
}

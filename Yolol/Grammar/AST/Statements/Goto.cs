using System;
using Yolol.Execution;
using Yolol.Grammar.AST.Expressions;

using Type = Yolol.Execution.Type;

namespace Yolol.Grammar.AST.Statements
{
    public class Goto
        : BaseStatement, IEquatable<Goto>
    {
        public override bool CanRuntimeError => true;

        public BaseExpression Destination { get; }

        public Goto(BaseExpression destination)
        {
            Destination = destination;
        }

        public override ExecutionResult Evaluate(MachineState state)
        {
            var dest = Destination.Evaluate(state);

            if (dest.Type != Type.Number)
                throw new ExecutionException($"Attempted to goto to a value of type `{dest.Type}`");

            if (dest.Number < Number.One)
                dest = Number.One;
            if (dest.Number > (Number)state.MaxLineNumber)
                dest = (Number)state.MaxLineNumber;

            return new ExecutionResult((int)dest.Number);
        }

        public bool Equals(Goto? other)
        {
            return other != null
                && other.Destination.Equals(Destination);
        }

        public override bool Equals(BaseStatement? other)
        {
            return other is Goto @goto
                   && @goto.Equals(this);
        }

        public override string ToString()
        {
            return $"goto {Destination}";
        }

        public override int GetHashCode()
        {
            return Destination.GetHashCode();
        }
    }
}

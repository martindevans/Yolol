using System;
using JetBrains.Annotations;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class Sqrt
        : BaseExpression
    {
        [NotNull] public BaseExpression Parameter { get; }

        public override bool CanRuntimeError => true;

        public override bool IsBoolean => false;

        public override bool IsConstant => Parameter.IsConstant;

        public Sqrt([NotNull] BaseExpression parameter)
        {
            Parameter = parameter;
        }

        public override Value Evaluate(MachineState state)
        {
            var input = Parameter.Evaluate(state);

            if (input.Type == Execution.Type.String)
                throw new ExecutionException("Attempted to Sqrt a string value");

            if (input.Number < 0)
                throw new ExecutionException("Attempted to Sqrt a negative value");

            return (decimal)Math.Sqrt((double)input.Number.Value);
        }

        public bool Equals([CanBeNull] Sqrt other)
        {
            return other != null 
                && other.Parameter.Equals(Parameter);
        }

        public override bool Equals(BaseExpression other)
        {
            return other is Sqrt sqrt
                && sqrt.Equals(this);
        }

        public override string ToString()
        {
            return $"SQRT({Parameter})";
        }
    }
}

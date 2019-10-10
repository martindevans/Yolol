using System;
using JetBrains.Annotations;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class Abs
        : BaseExpression
    {
        [NotNull] public BaseExpression Parameter { get; }

        public override bool CanRuntimeError => true;

        public override bool IsBoolean => false;

        public override bool IsConstant => Parameter.IsConstant;

        public Abs([NotNull] BaseExpression parameter)
        {
            Parameter = parameter;
        }

        public override Value Evaluate(MachineState state)
        {
            var input = Parameter.Evaluate(state);

            if (input.Type == Execution.Type.String)
                throw new ExecutionException("Attempted to Abs a string value");

            if (input.Number < 0)
                return new Value(-input.Number);
            else
                return input;
        }

        public bool Equals([CanBeNull] Abs other)
        {
            return other != null 
                && other.Parameter.Equals(Parameter);
        }

        public override bool Equals(BaseExpression other)
        {
            return other is Abs abs
                && abs.Equals(this);
        }

        public override string ToString()
        {
            return $"ABS({Parameter})";
        }
    }
}

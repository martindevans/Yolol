using System;
using JetBrains.Annotations;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class Sqrt
        : BaseUnaryExpression
    {
        public override bool CanRuntimeError => true;

        public override bool IsBoolean => false;

        public override bool IsConstant => Parameter.IsConstant;

        public Sqrt([NotNull] BaseExpression parameter): base(parameter) { }

        protected override Value Evaluate([NotNull] string parameterValue)
        {
            throw new ExecutionException("Attempted to Sqrt a string value");
        }

        protected override Value Evaluate(Number parameterValue)
        {
            if (parameterValue < 0)
                throw new ExecutionException("Attempted to Sqrt a negative value");

            return (decimal)Math.Sqrt((double)parameterValue.Value);
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

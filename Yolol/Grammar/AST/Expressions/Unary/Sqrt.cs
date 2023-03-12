using System;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class Sqrt
        : BaseUnaryExpression, IEquatable<Sqrt>
    {
        public override bool CanRuntimeError => true;

        public override bool IsBoolean => false;

        public override bool IsConstant => Parameter.IsConstant;

        public Sqrt(BaseExpression parameter)
            : base(parameter)
        {
        }

        protected override Value Evaluate(Value value, int maxStringLength) => Value.Sqrt(value);

        public bool Equals(Sqrt? other)
        {
            return other != null 
                && other.Parameter.Equals(Parameter);
        }

        public override bool Equals(BaseExpression? other)
        {
            return other is Sqrt sqrt
                && sqrt.Equals(this);
        }

        public override string ToString()
        {
            return $"SQRT {Parameter}";
        }
    }
}

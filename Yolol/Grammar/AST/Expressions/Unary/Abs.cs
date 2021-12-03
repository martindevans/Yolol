using System;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class Abs
        : BaseUnaryExpression, IEquatable<Abs>
    {
        public override bool CanRuntimeError => true;

        public override bool IsBoolean => false;

        public Abs(BaseExpression parameter)
            : base(parameter)
        {
        }

        protected override Value Evaluate(Value value, int _) => Value.Abs(value);

        public bool Equals(Abs? other)
        {
            return other != null 
                && other.Parameter.Equals(Parameter);
        }

        public override bool Equals(BaseExpression? other)
        {
            return other is Abs abs
                && abs.Equals(this);
        }

        public override string ToString()
        {
            return $"ABS {Parameter}";
        }
    }
}

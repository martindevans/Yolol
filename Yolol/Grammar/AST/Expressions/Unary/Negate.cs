using System;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class Negate
        : BaseUnaryExpression, IEquatable<Negate>
    {
        public override bool CanRuntimeError => Parameter.CanRuntimeError;

        public override bool IsBoolean => false;

        public Negate(BaseExpression parameter)
            : base(parameter)
        {
        }

        protected override Value Evaluate(Value value) => -value;

        public bool Equals(Negate? other)
        {
            return other != null
                && other.Parameter.Equals(Parameter);
        }

        public override bool Equals(BaseExpression? other)
        {
            return other is Negate neg
                && neg.Equals(this);
        }

        public override string ToString()
        {
            return $"-{Parameter}";
        }
    }
}

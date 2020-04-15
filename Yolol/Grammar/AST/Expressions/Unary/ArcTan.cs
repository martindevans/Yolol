using System;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class ArcTan
        : BaseTrigonometry, IEquatable<ArcTan>
    {
        public ArcTan(BaseExpression parameter)
            : base(parameter, "atan")
        {
        }

        protected override Value Evaluate(Value value)
        {
            return Value.ArcTan(value);
        }

        public bool Equals(ArcTan? other)
        {
            return other != null 
                && other.Parameter.Equals(Parameter);
        }

        public override bool Equals(BaseExpression? other)
        {
            return other is ArcTan atan
                && atan.Equals(this);
        }
    }
}

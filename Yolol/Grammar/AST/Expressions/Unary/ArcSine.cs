using System;
using JetBrains.Annotations;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class ArcSine
        : BaseTrigonometry, IEquatable<ArcSine>
    {
        public ArcSine([NotNull] BaseExpression parameter)
            : base(parameter, "asin")
        {
        }

        protected override Value Evaluate(Value value)
        {
            return Value.ArcSin(value);
        }

        public bool Equals(ArcSine other)
        {
            return other != null 
                && other.Parameter.Equals(Parameter);
        }

        public override bool Equals(BaseExpression other)
        {
            return other is ArcSine asin
                && asin.Equals(this);
        }
    }
}

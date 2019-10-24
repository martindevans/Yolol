using System;
using JetBrains.Annotations;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class ArcSine
        : BaseTrigonometry, IEquatable<ArcSine>
    {
        public ArcSine([NotNull] BaseExpression parameter)
            : base(parameter, "asin", false, true)
        {
        }

        protected override double Evaluate(double value)
        {
            return Math.Asin(value);
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

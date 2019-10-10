using System;
using JetBrains.Annotations;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class ArcTan
        : BaseTrigonometry
    {
        public ArcTan([NotNull] BaseExpression parameter)
            : base(parameter, "atan", false, true)
        {
        }

        protected override double Evaluate(double value)
        {
            return Math.Atan(value);
        }

        public bool Equals([CanBeNull] ArcTan other)
        {
            return other != null 
                && other.Parameter.Equals(Parameter);
        }

        public override bool Equals(BaseExpression other)
        {
            return other is ArcTan atan
                && atan.Equals(this);
        }
    }
}

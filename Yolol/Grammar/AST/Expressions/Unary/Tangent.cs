using System;
using JetBrains.Annotations;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class Tangent
        : BaseTrigonometry, IEquatable<Tangent>
    {
        public Tangent([NotNull] BaseExpression parameter)
            : base(parameter, "tan", true, false)
        {
        }

        protected override double Evaluate(double value)
        {
            return Math.Tan(value);
        }

        public bool Equals(Tangent other)
        {
            return other != null 
                && other.Parameter.Equals(Parameter);
        }

        public override bool Equals(BaseExpression other)
        {
            return other is Tangent tan
                && tan.Equals(this);
        }
    }
}

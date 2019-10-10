using System;
using JetBrains.Annotations;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class Tangent
        : BaseTrigonometry
    {
        public Tangent([NotNull] BaseExpression parameter)
            : base(parameter, "tan", true, false)
        {
        }

        protected override double Evaluate(double value)
        {
            return Math.Tan(value);
        }

        public bool Equals([CanBeNull] Tangent other)
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

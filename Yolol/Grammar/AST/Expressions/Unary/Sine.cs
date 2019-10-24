using System;
using JetBrains.Annotations;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class Sine
        : BaseTrigonometry, IEquatable<Sine>
    {
        public Sine([NotNull] BaseExpression parameter)
            : base(parameter, "sin", true, false)
        {
        }

        protected override double Evaluate(double value)
        {
            return Math.Sin(value);
        }

        public bool Equals(Sine other)
        {
            return other != null 
                && other.Parameter.Equals(Parameter);
        }

        public override bool Equals(BaseExpression other)
        {
            return other is Sine sine
                && sine.Equals(this);
        }
    }
}

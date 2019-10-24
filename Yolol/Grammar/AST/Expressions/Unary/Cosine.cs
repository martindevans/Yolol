using System;
using JetBrains.Annotations;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class Cosine
        : BaseTrigonometry, IEquatable<Cosine>
    {
        public Cosine([NotNull] BaseExpression parameter)
            : base(parameter, "cos", true, false)
        {
        }

        protected override double Evaluate(double value)
        {
            return Math.Cos(value);
        }

        public bool Equals(Cosine other)
        {
            return other != null 
                && other.Parameter.Equals(Parameter);
        }

        public override bool Equals(BaseExpression other)
        {
            return other is Cosine cos
                && cos.Equals(this);
        }
    }
}

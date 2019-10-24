using System;
using JetBrains.Annotations;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class ArcCos
        : BaseTrigonometry, IEquatable<ArcCos>
    {
        public ArcCos([NotNull] BaseExpression parameter)
            : base(parameter, "acos", false, true)
        {
        }

        protected override double Evaluate(double value)
        {
            return Math.Acos(value);
        }

        public bool Equals(ArcCos other)
        {
            return other != null 
                && other.Parameter.Equals(Parameter);
        }

        public override bool Equals(BaseExpression other)
        {
            return other is ArcCos acos
                && acos.Equals(this);
        }
    }
}

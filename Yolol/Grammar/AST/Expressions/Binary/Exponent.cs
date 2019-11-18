using System;
using JetBrains.Annotations;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Binary
{
    public class Exponent
        : BaseBinaryExpression, IEquatable<Exponent>
    {
        public override bool CanRuntimeError => true;

        public Exponent([NotNull] BaseExpression left, [NotNull] BaseExpression right)
            : base(left, right)
        {
        }

        protected override Value Evaluate(Value l, Value r)
        {
            return Value.Exponent(l, r);
        }

        public bool Equals(Exponent other)
        {
            return other != null
                   && other.Left.Equals(Left)
                   && other.Right.Equals(Right);
        }

        public override bool Equals(BaseExpression other)
        {
            return other is Exponent a
                   && a.Equals(this);
        }

        public override string ToString()
        {
            return $"{Left}^{Right}";
        }
    }
}

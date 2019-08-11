using System;
using JetBrains.Annotations;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Binary
{
    public class NotEqualTo
        : BaseBinaryExpression, IEquatable<NotEqualTo>
    {
        public override bool CanRuntimeError => Left.CanRuntimeError || Right.CanRuntimeError;

        public override bool IsBoolean => true;

        public NotEqualTo([NotNull] BaseExpression lhs, [NotNull] BaseExpression rhs)
            : base(lhs, rhs)
        {
        }

        protected override Value Evaluate(string l, string r)
        {
            return new Value(l.Equals(r, StringComparison.OrdinalIgnoreCase) ? 0 : 1);
        }

        protected override Value Evaluate(Number l, Number r)
        {
            return new Value(l == r ? 0 : 1);
        }

        protected override Value Evaluate(string l, Number r)
        {
            return 1;
        }

        protected override Value Evaluate(Number l, string r)
        {
            return 1;
        }

        public bool Equals([CanBeNull] NotEqualTo other)
        {
            return other != null
                && other.Left.Equals(Left)
                && other.Right.Equals(Right);
        }

        public override bool Equals(BaseExpression other)
        {
            return other is NotEqualTo a
                && a.Equals(this);
        }

        public override string ToString()
        {
            return $"{Left}!={Right}";
        }
    }
}

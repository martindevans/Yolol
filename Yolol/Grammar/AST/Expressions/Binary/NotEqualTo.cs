using System;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Binary
{
    public class NotEqualTo
        : BaseBinaryExpression, IEquatable<NotEqualTo>
    {
        public override bool CanRuntimeError => Left.CanRuntimeError || Right.CanRuntimeError;

        public override bool IsBoolean => true;

        public NotEqualTo(BaseExpression lhs, BaseExpression rhs)
            : base(lhs, rhs)
        {
        }

        protected override Value Evaluate(Value l, Value r, int maxStringLength)
        {
            return new Value(l != r);
        }

        public bool Equals(NotEqualTo? other)
        {
            return other != null
                && other.Left.Equals(Left)
                && other.Right.Equals(Right);
        }

        public override bool Equals(BaseExpression? other)
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

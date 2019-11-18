using System;
using JetBrains.Annotations;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Binary
{
    public class LessThanEqualTo
        : BaseBinaryExpression, IEquatable<LessThanEqualTo>
    {
        public override bool CanRuntimeError => Left.CanRuntimeError || Right.CanRuntimeError;

        public override bool IsBoolean => true;

        public LessThanEqualTo([NotNull] BaseExpression lhs, [NotNull] BaseExpression rhs)
            : base(lhs, rhs)
        {
        }

        protected override Value Evaluate(Value l, Value r)
        {
            return l <= r;
        }

        public bool Equals(LessThanEqualTo other)
        {
            return other != null
                   && other.Left.Equals(Left)
                   && other.Right.Equals(Right);
        }

        public override bool Equals(BaseExpression other)
        {
            return other is LessThanEqualTo a
                   && a.Equals(this);
        }

        public override string ToString()
        {
            return $"{Left}<={Right}";
        }
    }
}

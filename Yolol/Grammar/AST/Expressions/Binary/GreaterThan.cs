using System;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Binary
{
    public class GreaterThan
        : BaseBinaryExpression, IEquatable<GreaterThan>
    {
        public override bool CanRuntimeError => Left.CanRuntimeError || Right.CanRuntimeError;

        public override bool IsBoolean => true;

        public GreaterThan(BaseExpression lhs, BaseExpression rhs)
            : base(lhs, rhs)
        {
        }

        protected override Value Evaluate(Value l, Value r)
        {
            return l > r;
        }

        public bool Equals(GreaterThan? other)
        {
            return other != null
                   && other.Left.Equals(Left)
                   && other.Right.Equals(Right);
        }

        public override bool Equals(BaseExpression? other)
        {
            return other is GreaterThan a
                   && a.Equals(this);
        }

        public override string ToString()
        {
            return $"{Left}>{Right}";
        }
    }
}

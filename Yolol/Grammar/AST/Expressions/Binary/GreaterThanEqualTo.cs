using System;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Binary
{
    public class GreaterThanEqualTo
        : BaseBinaryExpression, IEquatable<GreaterThanEqualTo>
    {
        public override bool CanRuntimeError => Left.CanRuntimeError || Right.CanRuntimeError;

        public override bool IsBoolean => true;

        public GreaterThanEqualTo(BaseExpression lhs, BaseExpression rhs)
            : base(lhs, rhs)
        {
        }

        protected override Value Evaluate(Value l, Value r)
        {
            return l >= r;
        }

        public bool Equals(GreaterThanEqualTo? other)
        {
            return other != null
                   && other.Left.Equals(Left)
                   && other.Right.Equals(Right);
        }

        public override bool Equals(BaseExpression? other)
        {
            return other is GreaterThanEqualTo a
                   && a.Equals(this);
        }

        public override string ToString()
        {
            return $"{Left}>={Right}";
        }
    }
}

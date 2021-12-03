using System;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Binary
{
    public class Subtract
        : BaseBinaryExpression, IEquatable<Subtract>
    {
        public override bool CanRuntimeError => Left.CanRuntimeError || Right.CanRuntimeError;

        public Subtract(BaseExpression left, BaseExpression right)
            : base(left, right)
        {
        }

        protected override Value Evaluate(Value l, Value r, int _)
        {
            return l - r;
        }

        public bool Equals(Subtract? other)
        {
            return other != null
                   && other.Left.Equals(Left)
                   && other.Right.Equals(Right);
        }

        public override bool Equals(BaseExpression? other)
        {
            return other is Subtract a
                && a.Equals(this);
        }

        public override string ToString()
        {
            return $"{Left}-{Right}";
        }
    }
}

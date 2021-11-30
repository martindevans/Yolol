using System;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Binary
{
    public class Add
        : BaseBinaryExpression, IEquatable<Add>
    {
        public override bool CanRuntimeError => Left.CanRuntimeError || Right.CanRuntimeError;

        public Add(BaseExpression left, BaseExpression right)
            : base(left, right)
        {
        }

        protected override Value Evaluate(Value l, Value r)
        {
            return l + r;
        }

        public bool Equals(Add? other)
        {
            return other != null
                && other.Left.Equals(Left)
                && other.Right.Equals(Right);
        }

        public override bool Equals(BaseExpression? other)
        {
            return other is Add a
                && a.Equals(this);
        }

        public override string ToString()
        {
            return $"{Left}+{Right}";
        }
    }
}

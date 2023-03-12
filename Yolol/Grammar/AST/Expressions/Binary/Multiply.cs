using System;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Binary
{
    public class Multiply
        : BaseBinaryExpression, IEquatable<Multiply>
    {
        public override bool CanRuntimeError => true;

        public Multiply(BaseExpression left, BaseExpression right)
            : base(left, right)
        {
        }

        protected override Value Evaluate(Value l, Value r, int maxStringLength)
        {
            return l * r;
        }

        public bool Equals(Multiply? other)
        {
            return other != null
                   && other.Left.Equals(Left)
                   && other.Right.Equals(Right);
        }

        public override bool Equals(BaseExpression? other)
        {
            return other is Multiply a
                   && a.Equals(this);
        }

        public override string ToString()
        {
            return $"{Left}*{Right}";
        }
    }
}

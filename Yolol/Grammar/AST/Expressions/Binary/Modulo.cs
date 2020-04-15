using System;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Binary
{
    public class Modulo
        : BaseBinaryExpression, IEquatable<Modulo>
    {
        public override bool CanRuntimeError => true;

        public Modulo(BaseExpression left, BaseExpression right)
            : base(left, right)
        {
        }

        protected override Value Evaluate(Value l, Value r)
        {
            return l % r;
        }

        public bool Equals(Modulo? other)
        {
            return other != null
                   && other.Left.Equals(Left)
                   && other.Right.Equals(Right);
        }

        public override bool Equals(BaseExpression? other)
        {
            return other is Modulo a
                   && a.Equals(this);
        }

        public override string ToString()
        {
            return $"{Left}%{Right}";
        }
    }
}

using System;
using JetBrains.Annotations;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Binary
{
    public class Subtract
        : BaseBinaryExpression, IEquatable<Subtract>
    {
        public override bool CanRuntimeError => Left.CanRuntimeError || Right.CanRuntimeError;

        public Subtract([NotNull] BaseExpression left, [NotNull] BaseExpression right)
            : base(left, right)
        {
        }

        protected override Value Evaluate(string l, string r)
        {
            var index = l.LastIndexOf(r, StringComparison.Ordinal);

            if (index == -1)
                return new Value(l);
            else
                return new Value(l.Remove(index, r.Length));
        }

        protected override Value Evaluate(Number l, Number r)
        {
            return new Value(l - r);
        }

        protected override Value Evaluate(string l, Number r)
        {
            return Evaluate(l, r.ToString());
        }

        protected override Value Evaluate(Number l, string r)
        {
            return Evaluate(l.ToString(), r);
        }

        public bool Equals([CanBeNull] Subtract other)
        {
            return other != null
                   && other.Left.Equals(Left)
                   && other.Right.Equals(Right);
        }

        public override bool Equals(BaseExpression other)
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

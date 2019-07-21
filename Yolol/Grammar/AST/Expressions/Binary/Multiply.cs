using System;
using JetBrains.Annotations;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Binary
{
    public class Multiply
        : BaseBinaryExpression, IEquatable<Multiply>
    {
        public override bool CanRuntimeError => true;

        public Multiply([NotNull] BaseExpression left, [NotNull] BaseExpression right)
            : base(left, right)
        {
        }

        protected override Value Evaluate(string l, string r)
        {
            throw new ExecutionException("Attempted to multiply strings");
        }

        protected override Value Evaluate(Number l, Number r)
        {
            return new Value(l * r);
        }

        protected override Value Evaluate(string l, Number r)
        {
            throw new ExecutionException("Attempted to multiply mixed types");
        }

        protected override Value Evaluate(Number l, string r)
        {
            throw new ExecutionException("Attempted to multiply mixed types");
        }

        public bool Equals([CanBeNull] Multiply other)
        {
            return other != null
                   && other.Left.Equals(Left)
                   && other.Right.Equals(Right);
        }

        public override bool Equals(BaseExpression other)
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

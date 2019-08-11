using System;
using JetBrains.Annotations;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Binary
{
    public class Divide
        : BaseBinaryExpression, IEquatable<Divide>
    {
        public override bool CanRuntimeError => true;

        public Divide([NotNull] BaseExpression left, [NotNull] BaseExpression right)
            : base(left, right)
        {
        }

        protected override Value Evaluate(string l, string r)
        {
            throw new ExecutionException("Attempted to divide strings");
        }

        protected override Value Evaluate(Number l, Number r)
        {
            if (r == 0)
                throw new ExecutionException("Divide by zero");

            return new Value(l / r);
        }

        protected override Value Evaluate(string l, Number r)
        {
            throw new ExecutionException("Attempted to divide mixed types");
        }

        protected override Value Evaluate(Number l, string r)
        {
            throw new ExecutionException("Attempted to divide mixed types");
        }

        public bool Equals([CanBeNull] Divide other)
        {
            return other != null
                   && other.Left.Equals(Left)
                   && other.Right.Equals(Right);
        }

        public override bool Equals(BaseExpression other)
        {
            return other is Divide a
                && a.Equals(this);
        }

        public override string ToString()
        {
            return $"{Left}/{Right}";
        }
    }
}

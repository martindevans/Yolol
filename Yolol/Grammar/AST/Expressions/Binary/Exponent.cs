using System;
using JetBrains.Annotations;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Binary
{
    public class Exponent
        : BaseBinaryExpression, IEquatable<Exponent>
    {
        public override bool CanRuntimeError => true;

        public Exponent([NotNull] BaseExpression left, [NotNull] BaseExpression right)
            : base(left, right)
        {
        }

        protected override Value Evaluate(string l, string r)
        {
            throw new ExecutionException("Attempted to exponent strings");
        }

        protected override Value Evaluate(Number l, Number r)
        {
            var v = Math.Pow((double)l.Value, (double)r.Value);

            if (double.IsPositiveInfinity(v))
                return new Value(Number.MaxValue);

            if (double.IsNegativeInfinity(v) || double.IsNaN(v))
                return new Value(Number.MinValue);

            return new Value((decimal)v);
        }

        protected override Value Evaluate(string l, Number r)
        {
            throw new ExecutionException("Attempted to exponent mixed types");
        }

        protected override Value Evaluate(Number l, string r)
        {
            throw new ExecutionException("Attempted to exponent mixed types");
        }

        public bool Equals([CanBeNull] Exponent other)
        {
            return other != null
                   && other.Left.Equals(Left)
                   && other.Right.Equals(Right);
        }

        public override bool Equals(BaseExpression other)
        {
            return other is Exponent a
                   && a.Equals(this);
        }

        public override string ToString()
        {
            return $"{Left}^{Right}";
        }
    }
}

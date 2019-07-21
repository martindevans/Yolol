using System;
using JetBrains.Annotations;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Binary
{
    public class Modulo
        : BaseBinaryExpression, IEquatable<Modulo>
    {
        public override bool CanRuntimeError => true;

        public Modulo([NotNull] BaseExpression left, [NotNull] BaseExpression right)
            : base(left, right)
        {
        }

        protected override Value Evaluate(string l, string r)
        {
            throw new ExecutionException("Attempted to modulo strings");
        }

        protected override Value Evaluate(Number l, Number r)
        {
            return new Value(l.Value % r.Value);
        }

        protected override Value Evaluate(string l, Number r)
        {
            throw new ExecutionException("Attempted to modulo mixed types");
        }

        protected override Value Evaluate(Number l, string r)
        {
            throw new ExecutionException("Attempted to modulo mixed types");
        }

        public bool Equals([CanBeNull] Modulo other)
        {
            return other != null
                   && other.Left.Equals(Left)
                   && other.Right.Equals(Right);
        }

        public override bool Equals(BaseExpression other)
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

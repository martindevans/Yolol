using System;
using JetBrains.Annotations;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Binary
{
    public class Or
        : BaseBinaryExpression, IEquatable<Or>
    {
        public override bool CanRuntimeError => Left.CanRuntimeError || Right.CanRuntimeError;

        public override bool IsBoolean => true;

        public Or([NotNull] BaseExpression left, [NotNull] BaseExpression right)
            : base(left, right)
        {
        }

        public bool Equals([CanBeNull] Or other)
        {
            return other != null
                   && other.Left.Equals(Left)
                   && other.Right.Equals(Right);
        }

        public override bool Equals(BaseExpression other)
        {
            return other is Or a
                   && a.Equals(this);
        }

        public override string ToString()
        {
            return $"{Left} or {Right}";
        }

        protected override Value Evaluate(string l, string r)
        {
            return new Value(true);
        }

        protected override Value Evaluate(Number l, Number r)
        {
            return new Value(l != 0 || r != 0);
        }

        protected override Value Evaluate(string l, Number r)
        {
            return new Value(true);
        }

        protected override Value Evaluate(Number l, string r)
        {
            return new Value(true);
        }
    }
}

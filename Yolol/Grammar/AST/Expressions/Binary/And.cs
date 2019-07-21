using System;
using JetBrains.Annotations;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Binary
{
    public class And
        : BaseBinaryExpression, IEquatable<And>
    {
        public override bool CanRuntimeError => Left.CanRuntimeError || Right.CanRuntimeError;

        public override bool IsBoolean => true;

        public And([NotNull] BaseExpression left, [NotNull] BaseExpression right)
            : base(left, right)
        {
        }

        public bool Equals([CanBeNull] And other)
        {
            return other != null
                && other.Left.Equals(Left)
                && other.Right.Equals(Right);
        }

        public override bool Equals(BaseExpression other)
        {
            return other is And a
                && a.Equals(this);
        }

        public override string ToString()
        {
            return $"{Left} and {Right}";
        }

        protected override Value Evaluate(string l, string r)
        {
            return new Value(true);
        }

        protected override Value Evaluate(Number l, Number r)
        {
            return new Value(l != 0 && r != 0);
        }

        protected override Value Evaluate(string l, Number r)
        {
            return new Value(r != 0);
        }

        protected override Value Evaluate(Number l, string r)
        {
            return new Value(l != 0);
        }
    }
}

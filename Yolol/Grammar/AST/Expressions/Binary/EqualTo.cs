using System;
using JetBrains.Annotations;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Binary
{
    public class EqualTo
        : BaseBinaryExpression, IEquatable<EqualTo>
    {
        public override bool CanRuntimeError => Left.CanRuntimeError || Right.CanRuntimeError;

        public override bool IsBoolean => true;

        public EqualTo([NotNull] BaseExpression lhs, [NotNull] BaseExpression rhs)
            : base(lhs, rhs)
        {
        }

        protected override Value Evaluate(string l, string r)
        {
            return new Value(l.Equals(r, StringComparison.OrdinalIgnoreCase) ? 1 : 0);
        }

        protected override Value Evaluate(Number l, Number r)
        {
            return new Value(l == r ? 1 : 0);
        }

        protected override Value Evaluate(string l, Number r)
        {
            return 0;
        }

        protected override Value Evaluate(Number l, string r)
        {
            return 0;
        }

        public bool Equals([CanBeNull] EqualTo other)
        {
            return other != null
                   && other.Left.Equals(Left)
                   && other.Right.Equals(Right);
        }

        public override bool Equals(BaseExpression other)
        {
            return other is EqualTo a
                   && a.Equals(this);
        }

        public override string ToString()
        {
            return $"{Left}=={Right}";
        }
    }
}

using System;
using YololEmulator.Execution;
using YololEmulator.Execution.Extensions;

namespace YololEmulator.Grammar.AST.Expressions.Binary
{
    public class NotEqualToExpression
        : BaseBinaryExpression
    {
        public NotEqualToExpression(BaseExpression lhs, BaseExpression rhs)
            : base(lhs, rhs)
        {
        }

        protected override Value Evaluate(string l, string r)
        {
            return new Value(l.Equals(r, StringComparison.OrdinalIgnoreCase) ? 0 : 1);
        }

        protected override Value Evaluate(decimal l, decimal r)
        {
            return new Value(l == r ? 0 : 1);
        }

        protected override Value Evaluate(string l, decimal r)
        {
            return Evaluate(l, r.Coerce());
        }

        protected override Value Evaluate(decimal l, string r)
        {
            return Evaluate(l.Coerce(), r);
        }
    }
}

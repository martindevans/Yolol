using System;
using YololEmulator.Execution;
using YololEmulator.Execution.Extensions;

namespace YololEmulator.Grammar.AST.Expressions.Binary
{
    public class LessThanEqualToExpression
        : BaseBinaryExpression
    {
        public LessThanEqualToExpression(BaseExpression lhs, BaseExpression rhs)
            : base(lhs, rhs)
        {
        }

        protected override Value Evaluate(string l, string r)
        {
            var comparison = StringComparer.OrdinalIgnoreCase.Compare(l, r);

            return new Value(comparison <= 0 ? 1 : 0);
        }

        protected override Value Evaluate(decimal l, decimal r)
        {
            return new Value(l <= r ? 1 : 0);
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

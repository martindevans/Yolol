using System;
using YololEmulator.Execution;


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

        protected override Value Evaluate(Number l, Number r)
        {
            return new Value(l <= r ? 1 : 0);
        }

        protected override Value Evaluate(string l, Number r)
        {
            return Evaluate(l, r.ToString());
        }

        protected override Value Evaluate(Number l, string r)
        {
            return Evaluate(l.ToString(), r);
        }
    }
}

using System;
using YololEmulator.Execution;

namespace YololEmulator.Grammar.AST.Expressions.Binary
{
    internal class SubtractExpression
        : BaseBinaryExpression
    {
        public SubtractExpression(BaseExpression left, BaseExpression right)
            : base(left, right)
        {
        }

        protected override Value Evaluate(string l, string r)
        {
            var index = l.LastIndexOf(r, StringComparison.Ordinal);

            if (index == -1)
                return new Value(l);
            else
                return new Value(l.Remove(index, r.Length));
        }

        protected override Value Evaluate(decimal l, decimal r)
        {
            return new Value(l - r);
        }

        protected override Value Evaluate(string l, decimal r)
        {
            return Evaluate(l, r.ToString("#.####"));
        }

        protected override Value Evaluate(decimal l, string r)
        {
            return Evaluate(l.ToString("#.####"), r);
        }
    }
}

using System;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Binary
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

        protected override Value Evaluate(Number l, Number r)
        {
            return new Value(l - r);
        }

        protected override Value Evaluate(string l, Number r)
        {
            return Evaluate(l, r.ToString());
        }

        protected override Value Evaluate(Number l, string r)
        {
            return Evaluate(l.ToString(), r);
        }

        public override string ToString()
        {
            return $"{Left}-{Right}";
        }
    }
}

using System;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Binary
{
    public class Exponent
        : BaseBinaryExpression
    {
        public Exponent(BaseExpression left, BaseExpression right)
            : base(left, right)
        {
        }

        protected override Value Evaluate(string l, string r)
        {
            throw new ExecutionException("Attempted to exponent strings");
        }

        protected override Value Evaluate(Number l, Number r)
        {
            return new Value((decimal)Math.Pow((double)l.Value, (double)r.Value));
        }

        protected override Value Evaluate(string l, Number r)
        {
            throw new ExecutionException("Attempted to exponent mixed types");
        }

        protected override Value Evaluate(Number l, string r)
        {
            throw new ExecutionException("Attempted to exponent mixed types");
        }

        public override string ToString()
        {
            return $"{Left}^{Right}";
        }
    }
}

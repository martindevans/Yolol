using YololEmulator.Execution;

namespace YololEmulator.Grammar.AST.Expressions.Binary
{
    public class AddExpression
        : BaseBinaryExpression
    {
        public AddExpression(BaseExpression left, BaseExpression right)
            : base(left, right)
        {
        }

        protected override Value Evaluate(string l, string r)
        {
            return new Value(l + r);
        }

        protected override Value Evaluate(decimal l, decimal r)
        {
            return new Value(l + r);
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

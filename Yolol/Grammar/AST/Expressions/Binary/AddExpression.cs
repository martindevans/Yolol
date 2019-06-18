using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Binary
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

        protected override Value Evaluate(Number l, Number r)
        {
            return new Value(l + r);
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
            return $"{Left}+{Right}";
        }
    }
}

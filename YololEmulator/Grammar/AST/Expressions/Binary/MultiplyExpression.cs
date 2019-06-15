using YololEmulator.Execution;

namespace YololEmulator.Grammar.AST.Expressions.Binary
{
    public class MultiplyExpression
        : BaseBinaryExpression
    {
        public MultiplyExpression(BaseExpression left, BaseExpression right)
            : base(left, right)
        {
        }

        protected override Value Evaluate(string l, string r)
        {
            throw new ExecutionError("Attempted to multiply strings");
        }

        protected override Value Evaluate(decimal l, decimal r)
        {
            return new Value(l * r);
        }

        protected override Value Evaluate(string l, decimal r)
        {
            throw new ExecutionError("Attempted to multiply mixed types");
        }

        protected override Value Evaluate(decimal l, string r)
        {
            throw new ExecutionError("Attempted to multiply mixed types");
        }
    }
}

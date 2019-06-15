using YololEmulator.Execution;

namespace YololEmulator.Grammar.AST.Expressions.Binary
{
    public class DivideExpression
        : BaseBinaryExpression
    {
        public DivideExpression(BaseExpression left, BaseExpression right)
            : base(left, right)
        {
        }

        protected override Value Evaluate(string l, string r)
        {
            throw new ExecutionError("Attempted to divide strings");
        }

        protected override Value Evaluate(decimal l, decimal r)
        {
            return new Value(l / r);
        }

        protected override Value Evaluate(string l, decimal r)
        {
            throw new ExecutionError("Attempted to divide mixed types");
        }

        protected override Value Evaluate(decimal l, string r)
        {
            throw new ExecutionError("Attempted to divide mixed types");
        }
    }
}

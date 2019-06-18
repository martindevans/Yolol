using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class NegateExpression
        : BaseExpression
    {
        private readonly BaseExpression _expr;

        public NegateExpression(BaseExpression expr)
        {
            _expr = expr;
        }

        public override Value Evaluate(MachineState state)
        {
            var v = _expr.Evaluate(state);

            if (v.Type == Type.String)
                throw new ExecutionException("Attempted to negate a String value");

            return new Value(-v.Number);
        }

        public override string ToString()
        {
            return $"-{_expr}";
        }
    }
}

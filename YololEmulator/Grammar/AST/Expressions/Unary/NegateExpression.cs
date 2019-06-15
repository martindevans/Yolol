using YololEmulator.Execution;

namespace YololEmulator.Grammar.AST.Expressions.Unary
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

            if (v.Type == Type.None)
                throw new ExecutionError("Attempted to negate an unassigned value");

            if (v.Type == Type.String)
                throw new ExecutionError("Attempted to negate a String value");

            return new Value(-v.ValueNumber);
        }
    }
}

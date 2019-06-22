using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class NegateExpression
        : BaseExpression
    {
        public BaseExpression Expression { get; }

        public NegateExpression(BaseExpression expression)
        {
            Expression = expression;
        }

        public override Value Evaluate(MachineState state)
        {
            var v = Expression.Evaluate(state);

            if (v.Type == Type.String)
                throw new ExecutionException("Attempted to negate a String value");

            return new Value(-v.Number);
        }

        public override string ToString()
        {
            return $"-{Expression}";
        }
    }
}

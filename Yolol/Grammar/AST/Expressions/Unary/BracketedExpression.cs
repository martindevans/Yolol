using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class BracketedExpression
        : BaseExpression
    {
        private readonly BaseExpression _expr;

        public BracketedExpression(BaseExpression expr)
        {
            _expr = expr;
        }

        public override Value Evaluate(MachineState state)
        {
            return _expr.Evaluate(state);
        }

        public override string ToString()
        {
            return $"({_expr})";
        }
    }
}

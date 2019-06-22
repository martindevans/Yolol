using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class BracketedExpression
        : BaseExpression
    {
        public BaseExpression Expression { get; }

        public BracketedExpression(BaseExpression expression)
        {
            Expression = expression;
        }

        public override Value Evaluate(MachineState state)
        {
            return Expression.Evaluate(state);
        }

        public override string ToString()
        {
            return $"({Expression})";
        }
    }
}

using JetBrains.Annotations;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class Bracketed
        : BaseExpression
    {
        [NotNull] public BaseExpression Expression { get; }

        public override bool IsConstant => Expression.IsConstant;

        public Bracketed([NotNull] BaseExpression expression)
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

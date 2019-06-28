using JetBrains.Annotations;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class Negate
        : BaseExpression
    {
        [NotNull] public BaseExpression Expression { get; }

        public override bool IsBoolean => false;

        public override bool IsConstant => Expression.IsConstant;

        public Negate([NotNull] BaseExpression expression)
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

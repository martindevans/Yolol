using JetBrains.Annotations;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class Not
        : BaseExpression
    {
        [NotNull] public BaseExpression Expression { get; }

        public override bool CanRuntimeError => Expression.CanRuntimeError;

        public override bool IsBoolean => true;

        public override bool IsConstant => Expression.IsConstant;

        public Not([NotNull] BaseExpression expression)
        {
            Expression = expression;
        }

        public override Value Evaluate(MachineState state)
        {
            var v = Expression.Evaluate(state);

            if (v.Type == Type.String)
                return new Value(false);

            return new Value(v.Number == 0);
        }

        public override string ToString()
        {
            return $"not {Expression}";
        }
    }
}

using System;
using JetBrains.Annotations;
using Yolol.Execution;
using Type = Yolol.Execution.Type;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public abstract class BaseUnaryExpression
        : BaseExpression
    {
        [NotNull] public BaseExpression Parameter { get; }

        public override bool IsConstant => Parameter.IsConstant;

        protected BaseUnaryExpression([NotNull] BaseExpression parameter)
        {
            Parameter = parameter;
        }

        protected abstract Value Evaluate([NotNull] string parameterValue);

        protected abstract Value Evaluate(Number parameterValue);

        public override Value Evaluate(MachineState state)
        {
            var parameterValue = Parameter.Evaluate(state);

            switch (parameterValue.Type)
            {
                case Type.String:
                    return Evaluate(parameterValue.String);
                case Type.Number:
                    return Evaluate(parameterValue.Number);
                default:
                    throw new InvalidOperationException($"Unknown type `{parameterValue.Type}` encountered evaluating `{GetType().Name}`");
            }
        }
    }
}

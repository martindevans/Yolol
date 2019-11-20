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

        protected abstract Value Evaluate(Value value);

        public override Value Evaluate(MachineState state)
        {
            var value = Parameter.Evaluate(state);
            return Evaluate(value);
        }
    }
}

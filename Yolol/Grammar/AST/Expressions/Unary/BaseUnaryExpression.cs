using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;
using Yolol.Execution;
using Type = Yolol.Execution.Type;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public abstract class BaseUnaryExpression: BaseExpression
    {
        [NotNull] public BaseExpression Parameter { get; }

        public override bool IsConstant => Parameter.IsConstant;

        public override bool IsBoolean => false;

        protected BaseUnaryExpression([NotNull] BaseExpression parameter)
        {
            Parameter = parameter;
        }

        protected abstract Value Evaluate([NotNull] string parameterValue);

        protected abstract Value Evaluate(Number parameterValue);

        public override Value Evaluate([NotNull] MachineState state)
        {
            var parameterValue = Parameter.Evaluate(state);

            switch (parameterValue.Type)
            {
                case Type.String:
                    return Evaluate(parameterValue.String);
                default:
                    return Evaluate(parameterValue.Number);
            }
        }
    }
}

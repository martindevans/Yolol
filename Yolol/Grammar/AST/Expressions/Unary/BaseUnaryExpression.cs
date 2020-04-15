using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public abstract class BaseUnaryExpression
        : BaseExpression
    {
        public BaseExpression Parameter { get; }

        public override bool IsConstant => Parameter.IsConstant;

        protected BaseUnaryExpression(BaseExpression parameter)
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

using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public abstract class BaseModify
        : BaseExpression
    {
        public VariableName Name { get; }

        protected BaseModify(VariableName name)
        {
            Name = name;
        }

        protected abstract Value Modify(Value value);

        protected abstract Value Return(Value original, Value modified);

        public override Value Evaluate(MachineState state)
        {
            var variable = state.GetVariable(Name);

            var original = variable.Value;
            var modified = Modify(original);

            variable.Value = modified;

            return Return(original, modified);
        }
    }
}

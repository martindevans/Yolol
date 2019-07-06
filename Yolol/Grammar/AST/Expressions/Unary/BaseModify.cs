using JetBrains.Annotations;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public abstract class BaseModify
        : BaseExpression
    {
        [NotNull] public VariableName Name { get; }

        public override bool CanRuntimeError => false;

        public override bool IsBoolean => false;

        public override bool IsConstant => false;

        protected BaseModify([NotNull] VariableName name)
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

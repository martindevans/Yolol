using System;
using JetBrains.Annotations;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public abstract class BaseModifyInPlace
        : BaseExpression
    {
        [NotNull] public VariableName Name { get; }

        public override bool CanRuntimeError => false;

        public override bool IsBoolean => false;

        public override bool IsConstant => false;

        public YololModifyOp Op { get; }

        public bool PreOp { get; }

        protected BaseModifyInPlace([NotNull] VariableName name, YololModifyOp op, bool pre)
        {
            Name = name;
            Op = op;
            PreOp = pre;
        }

        protected abstract Value Modify(Value value);

        public override Value Evaluate(MachineState state)
        {
            var variable = state.GetVariable(Name);

            var original = variable.Value;
            var modified = Modify(original);

            variable.Value = modified;

            return PreOp ? modified : original;
        }

        [NotNull] public static BaseModifyInPlace Create([NotNull] VariableName name, YololModifyOp op, bool pre)
        {
            switch (op)
            {
                case YololModifyOp.Increment: return pre ? new PreIncrement(name) : (BaseModifyInPlace)new PostIncrement(name);
                case YololModifyOp.Decrement: return pre ? new PreDecrement(name) : (BaseModifyInPlace)new PostIncrement(name);

                default: throw new ArgumentException($"Unknown YololModifyOp type `{op}`");
            }
        }
    }
}

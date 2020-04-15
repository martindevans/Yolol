using System;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public abstract class BaseModifyInPlace
        : BaseUnaryExpression
    {
        public VariableName Name { get; }

        public override bool CanRuntimeError => false;

        public override bool IsBoolean => false;

        public override bool IsConstant => false;

        public YololModifyOp Op { get; }

        public bool PreOp { get; }

        protected BaseModifyInPlace(VariableName name, YololModifyOp op, bool pre): base(new Variable(name))
        {
            Name = name;
            Op = op;
            PreOp = pre;
        }

        public override Value Evaluate(MachineState state)
        {
            var variable = state.GetVariable(Name);

            var original = variable.Value;
            var modified = Evaluate(original);

            variable.Value = modified;

            return PreOp ? modified : original;
        }

        public static BaseModifyInPlace Create(VariableName name, YololModifyOp op, bool pre)
        {
            return op switch {
                YololModifyOp.Increment => (pre ? new PreIncrement(name) : (BaseModifyInPlace)new PostIncrement(name)),
                YololModifyOp.Decrement => (pre ? new PreDecrement(name) : (BaseModifyInPlace)new PostIncrement(name)),
                _ => throw new ArgumentException($"Unknown YololModifyOp type `{op}`")
            };
        }
    }
}

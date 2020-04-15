using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public abstract class BaseDecrement
        : BaseModifyInPlace
    {
        protected BaseDecrement(VariableName name, bool pre)
            : base(name, YololModifyOp.Decrement, pre)
        {
        }

        protected override Value Evaluate(Value value)
        {
            return --value;
        }
    }
}

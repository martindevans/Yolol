using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public abstract class BaseIncrement
        : BaseModifyInPlace
    {
        protected BaseIncrement(VariableName name, bool pre)
            : base(name, YololModifyOp.Increment, pre)
        {
        }

        protected override Value Evaluate(Value value)
        {
            return ++value;
        }
    }
}

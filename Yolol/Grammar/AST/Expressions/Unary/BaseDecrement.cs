using JetBrains.Annotations;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public abstract class BaseDecrement
        : BaseModifyInPlace
    {
        protected BaseDecrement([NotNull] VariableName name, bool pre)
            : base(name, YololModifyOp.Decrement, pre)
        {
        }

        protected override Value Evaluate(Value value)
        {
            if (value.Type == Type.Number)
                return new Value(value.Number - 1);

            if (value.String == "")
                throw new ExecutionException("Attempted to decrement empty string");
            return new Value(value.String.Substring(0, value.String.Length - 1));
        }
    }
}

using Yolol.Execution;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public abstract class BaseDecrement
        : BaseModify
    {
        protected BaseDecrement(VariableName name)
            : base(name)
        {
        }

        protected override Value Modify(Value value)
        {
            if (value.Type == Type.Number)
                return new Value(value.Number - 1);

            if (value.String == "")
                throw new ExecutionException("Attempted to decrement empty string");
            return new Value(value.String.Substring(0, value.String.Length - 1));
        }
    }
}

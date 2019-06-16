using YololEmulator.Execution;
using YololEmulator.Grammar.AST.Statements;

namespace YololEmulator.Grammar.AST.Expressions.Unary
{
    public abstract class BaseDecrement
        : BasePrePostModify
    {
        protected BaseDecrement(VariableName name)
            : base(name)
        {
        }

        protected override Value Modify(Value value)
        {
            if (value.Type == Type.Number)
                return new Value(value.Number - 1);

            if (value.Type == Type.String)
            {
                if (value.String == "")
                    throw new ExecutionException("Attempted to decrement empty string");
                return new Value(value.String.Substring(0, value.String.Length - 1));
            }

            throw new ExecutionException($"Attempted to increment a variable of type `{value.Type}`");
        }
    }
}

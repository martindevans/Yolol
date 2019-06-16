using YololEmulator.Execution;
using YololEmulator.Grammar.AST.Statements;

namespace YololEmulator.Grammar.AST.Expressions.Unary
{
    public abstract class BaseIncrement
        : BasePrePostModify
    {
        protected BaseIncrement(VariableName name)
            : base(name)
        {
        }

        protected override Value Modify(Value value)
        {
            if (value.Type == Type.Number)
                return new Value(value.Number + 1);

            if (value.Type == Type.String)
                return new Value(value.String + " ");

            throw new ExecutionError($"Attempted to increment a variable of type `{value.Type}`");
        }
    }
}

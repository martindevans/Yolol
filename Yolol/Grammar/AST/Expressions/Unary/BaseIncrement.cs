using Yolol.Execution;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public abstract class BaseIncrement
        : BaseModify
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

            //ncrunch: no coverage start
            throw new ExecutionException($"Attempted to increment a variable of type `{value.Type}`");
            //ncrunch: no coverage end
        }
    }
}

using JetBrains.Annotations;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public abstract class BaseIncrement
        : BaseModify
    {
        protected BaseIncrement([NotNull] VariableName name)
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

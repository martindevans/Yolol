using JetBrains.Annotations;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public abstract class BaseIncrement
        : BaseModifyInPlace
    {
        protected BaseIncrement([NotNull] VariableName name, bool pre)
            : base(name, YololModifyOp.Increment, pre)
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

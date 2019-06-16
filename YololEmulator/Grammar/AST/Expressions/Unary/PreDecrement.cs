using YololEmulator.Execution;
using YololEmulator.Grammar.AST.Statements;

namespace YololEmulator.Grammar.AST.Expressions.Unary
{
    public class PreDecrement
        : BaseDecrement
    {
        public PreDecrement(VariableName name)
            : base(name)
        {
        }

        protected override Value Return(Value original, Value modified)
        {
            return modified;
        }
    }
}

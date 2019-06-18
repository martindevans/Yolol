using Yolol.Execution;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Grammar.AST.Expressions.Unary
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

        public override string ToString()
        {
            return $"--{Name}";
        }
    }
}

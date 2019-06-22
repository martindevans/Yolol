using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class PreIncrement
        : BaseIncrement
    {
        public PreIncrement(VariableName name)
            : base(name)
        {
        }

        protected override Value Return(Value original, Value modified)
        {
            return modified;
        }

        public override string ToString()
        {
            return $"++{Name}";
        }
    }
}

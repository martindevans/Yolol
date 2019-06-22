using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class PostDecrement
        : BaseDecrement
    {
        public PostDecrement(VariableName name)
            : base(name)
        {
        }

        protected override Value Return(Value original, Value modified)
        {
            return original;
        }

        public override string ToString()
        {
            return $"{Name}--";
        }
    }
}

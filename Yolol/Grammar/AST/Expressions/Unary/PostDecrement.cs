using JetBrains.Annotations;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class PostDecrement
        : BaseDecrement
    {
        public PostDecrement([NotNull] VariableName name)
            : base(name, false)
        {
        }

        public override string ToString()
        {
            return $"{Name}--";
        }
    }
}

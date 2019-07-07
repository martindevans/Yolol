using JetBrains.Annotations;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class PreDecrement
        : BaseDecrement
    {
        public PreDecrement([NotNull] VariableName name)
            : base(name, true)
        {
        }

        public override string ToString()
        {
            return $"--{Name}";
        }
    }
}

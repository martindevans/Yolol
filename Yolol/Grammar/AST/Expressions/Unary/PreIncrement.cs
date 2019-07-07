using JetBrains.Annotations;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class PreIncrement
        : BaseIncrement
    {
        public PreIncrement([NotNull] VariableName name)
            : base(name, true)
        {
        }

        public override string ToString()
        {
            return $"++{Name}";
        }
    }
}

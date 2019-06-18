using Yolol.Execution;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class PostIncrement
        : BaseIncrement
    {
        public PostIncrement(VariableName name)
            : base(name)
        {
        }

        protected override Value Return(Value original, Value modified)
        {
            return original;
        }

        public override string ToString()
        {
            return $"{Name}++";
        }
    }
}

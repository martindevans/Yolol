using System;
using JetBrains.Annotations;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class PreIncrement
        : BaseIncrement, IEquatable<PreIncrement>
    {
        public PreIncrement([NotNull] VariableName name)
            : base(name, true)
        {
        }

        public bool Equals(PreIncrement other)
        {
            return other != null
                   && other.Name.Equals(Name);
        }

        public override bool Equals(BaseExpression other)
        {
            return other is PreIncrement pre
                && pre.Equals(this);
        }

        public override string ToString()
        {
            return $"++{Name}";
        }
    }
}

using System;
using JetBrains.Annotations;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class PreDecrement
        : BaseDecrement, IEquatable<PreDecrement>
    {
        public PreDecrement([NotNull] VariableName name)
            : base(name, true)
        {
        }

        public bool Equals(PreDecrement other)
        {
            return other != null
                && other.Name.Equals(Name);
        }

        public override bool Equals(BaseExpression other)
        {
            return other is PreDecrement pre
                && pre.Equals(this);
        }

        public override string ToString()
        {
            return $"--{Name}";
        }
    }
}

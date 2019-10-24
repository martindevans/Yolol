using System;
using JetBrains.Annotations;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class PostDecrement
        : BaseDecrement, IEquatable<PostDecrement>
    {
        public PostDecrement([NotNull] VariableName name)
            : base(name, false)
        {
        }

        public bool Equals(PostDecrement other)
        {
            return other != null
                   && other.Name.Equals(Name);
        }

        public override bool Equals(BaseExpression other)
        {
            return other is PostDecrement post
                   && post.Equals(this);
        }

        public override string ToString()
        {
            return $"{Name}--";
        }
    }
}

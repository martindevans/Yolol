using System;
using JetBrains.Annotations;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class PostIncrement
        : BaseIncrement, IEquatable<PostIncrement>
    {
        public PostIncrement([NotNull] VariableName name)
            : base(name, false)
        {
        }

        public bool Equals([CanBeNull] PostIncrement other)
        {
            return other != null
                && other.Name.Equals(Name);
        }

        public override bool Equals(BaseExpression other)
        {
            return other is PostIncrement post
                && post.Equals(this);
        }

        public override string ToString()
        {
            return $"{Name}++";
        }
    }
}

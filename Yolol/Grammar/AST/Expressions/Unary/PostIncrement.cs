using System;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class PostIncrement
        : BaseIncrement, IEquatable<PostIncrement>
    {
        public PostIncrement(VariableName name)
            : base(name, true)
        {
            //note: This currently specifies post increment to do a pre increment because that's how the game does it.
            //      If/When that's fixed: change the `true` above to a `false`
        }

        public bool Equals(PostIncrement? other)
        {
            return other != null
                && other.Name.Equals(Name);
        }

        public override bool Equals(BaseExpression? other)
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

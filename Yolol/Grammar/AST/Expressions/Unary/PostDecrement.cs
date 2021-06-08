using System;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class PostDecrement
        : BaseDecrement, IEquatable<PostDecrement>
    {
        public PostDecrement(VariableName name)
            : base(name, true)
        {
            //note: This currently specifies post decrement to do a pre decrement because that's how the game does it.
            //      If/When that's fixed: change the `true` above to a `false`
        }

        public bool Equals(PostDecrement? other)
        {
            return other != null
                   && other.Name.Equals(Name);
        }

        public override bool Equals(BaseExpression? other)
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

namespace Yolol.Grammar
{
    public enum YololToken
    {
        LessThan,
        GreaterThan,
        LessThanEqualTo,
        GreaterThanEqualTo,
        NotEqualTo,
        EqualTo,

        Assignment,

        CompoundPlus,
        CompoundSubtract,
        CompoundMultiply,
        CompoundDivide,

        Plus,
        Subtract,
        Multiply,
        Divide,

        Increment,
        Decrement,

        LParen,
        RParen,

        If,
        Then,
        Else,
        End,

        Goto,

        Identifier,
        ExternalIdentifier,

        Number,
        String,
        
    }
}

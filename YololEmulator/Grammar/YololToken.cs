namespace YololEmulator.Grammar
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

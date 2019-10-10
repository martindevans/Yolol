namespace Yolol.Grammar
{
    public enum YololToken
    {
        NewLine,

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
        CompoundModulo,

        Plus,
        Subtract,
        Multiply,
        Divide,
        Modulo,
        Exponent,

        Increment,
        Decrement,

        LParen,
        RParen,

        If,
        Then,
        Else,
        End,

        And,
        Or,
        Not,

        Goto,

        Identifier,
        ExternalIdentifier,

        Number,
        String,

        Abs,
        Sqrt,
        Sine,
        Cosine,
        Tangent,
        ArcSin,
        ArcCos,
        ArcTan,
    }
}

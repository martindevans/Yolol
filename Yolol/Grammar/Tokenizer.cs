using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace Yolol.Grammar
{
    public class Tokenizer
    {
        private static Tokenizer<YololToken> Instance { get; } = 
            new TokenizerBuilder<YololToken>()

                .Match(Span.Regex("\\r?\\n"), YololToken.NewLine)

                .Ignore(Span.WhiteSpace)
                .Ignore(Comment.CPlusPlusStyle)

                .Match(Span.EqualTo("if"), YololToken.If)
                .Match(Span.EqualTo("then"), YololToken.Then)
                .Match(Span.EqualTo("else"), YololToken.Else)
                .Match(Span.EqualTo("end"), YololToken.End)
                .Match(Span.EqualTo("goto"), YololToken.Goto)

                .Match(Span.EqualTo("and"), YololToken.And)
                .Match(Span.EqualTo("or"), YololToken.Or)
                .Match(Span.EqualTo("not"), YololToken.Not)

                .Match(Span.EqualToIgnoreCase("abs"), YololToken.Abs)
                .Match(Span.EqualToIgnoreCase("sqrt"), YololToken.Sqrt)
                .Match(Span.EqualToIgnoreCase("sin"), YololToken.Sine)
                .Match(Span.EqualToIgnoreCase("cos"), YololToken.Cosine)
                .Match(Span.EqualToIgnoreCase("tan"), YololToken.Tangent)
                .Match(Span.EqualToIgnoreCase("asin"), YololToken.ArcSin)
                .Match(Span.EqualToIgnoreCase("acos"), YololToken.ArcCos)
                .Match(Span.EqualToIgnoreCase("atan"), YololToken.ArcTan)

                .Match(Span.EqualTo("<="), YololToken.LessThanEqualTo)
                .Match(Span.EqualTo(">="), YololToken.GreaterThanEqualTo)
                .Match(Character.EqualTo('<'), YololToken.LessThan)
                .Match(Character.EqualTo('>'), YololToken.GreaterThan)

                .Match(Span.EqualTo("!="), YololToken.NotEqualTo)
                .Match(Span.EqualTo("=="), YololToken.EqualTo)

                .Match(Span.EqualTo(':').IgnoreThen(Identifier.CStyle), YololToken.ExternalIdentifier)
                .Match(Identifier.CStyle, YololToken.Identifier)
                .Match(QuotedString.CStyle, YololToken.String)

                .Match(Character.EqualTo('='), YololToken.Assignment)

                .Match(Character.EqualTo('('), YololToken.LParen)
                .Match(Character.EqualTo(')'), YololToken.RParen)

                .Match(Span.EqualTo("++"), YololToken.Increment)
                .Match(Span.EqualTo("--"), YololToken.Decrement)

                .Match(Span.EqualTo("+="), YololToken.CompoundPlus)
                .Match(Span.EqualTo("-="), YololToken.CompoundSubtract)
                .Match(Span.EqualTo("*="), YololToken.CompoundMultiply)
                .Match(Span.EqualTo("/="), YololToken.CompoundDivide)
                .Match(Span.EqualTo("%="), YololToken.CompoundModulo)

                .Match(Character.EqualTo('+'), YololToken.Plus)
                .Match(Character.EqualTo('-'), YololToken.Subtract)
                .Match(Character.EqualTo('*'), YololToken.Multiply)
                .Match(Character.EqualTo('/'), YololToken.Divide)
                .Match(Character.EqualTo('%'), YololToken.Modulo)
                .Match(Character.EqualTo('^'), YololToken.Exponent)

                .Match(Numerics.Decimal, YololToken.Number)

                .Build();

        public static Result<TokenList<YololToken>> TryTokenize(string str)
        {
            // Ensure the last line ends with a newline
            if (!str.EndsWith("\n"))
                str += "\n";

            return Instance.TryTokenize(str);
        }
    }
}

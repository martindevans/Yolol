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
                .Ignore(Span.WhiteSpace)
                .Ignore(Comment.CPlusPlusStyle)

                .Match(Span.EqualTo("<="), YololToken.LessThanEqualTo)
                .Match(Span.EqualTo(">="), YololToken.GreaterThanEqualTo)
                .Match(Character.EqualTo('<'), YololToken.LessThan)
                .Match(Character.EqualTo('>'), YololToken.GreaterThan)

                .Match(Span.EqualTo("!="), YololToken.NotEqualTo)
                .Match(Span.EqualTo("=="), YololToken.EqualTo)

                .Match(Span.EqualToIgnoreCase("if"), YololToken.If)
                .Match(Span.EqualToIgnoreCase("then"), YololToken.Then)
                .Match(Span.EqualToIgnoreCase("else"), YololToken.Else)
                .Match(Span.EqualToIgnoreCase("end"), YololToken.End)
                .Match(Span.EqualToIgnoreCase("goto"), YololToken.Goto)

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
            return Instance.TryTokenize(str);
        }
    }
}

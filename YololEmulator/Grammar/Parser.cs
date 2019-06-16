using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;
using YololEmulator.Grammar.AST.Expressions;
using YololEmulator.Grammar.AST.Expressions.Unary;
using YololEmulator.Grammar.AST.Statements;

namespace YololEmulator.Grammar
{
    public static class Parser
    {
        private static readonly TokenListParser<YololToken, YololBinaryOp> Add = Token.EqualTo(YololToken.Plus).Value(YololBinaryOp.Add);
        private static readonly TokenListParser<YololToken, YololBinaryOp> Subtract = Token.EqualTo(YololToken.Subtract).Value(YololBinaryOp.Subtract);
        private static readonly TokenListParser<YololToken, YololBinaryOp> Multiply = Token.EqualTo(YololToken.Multiply).Value(YololBinaryOp.Multiply);
        private static readonly TokenListParser<YololToken, YololBinaryOp> Divide = Token.EqualTo(YololToken.Divide).Value(YololBinaryOp.Divide);

        private static readonly TokenListParser<YololToken, YololBinaryOp> LessThan = Token.EqualTo(YololToken.LessThan).Value(YololBinaryOp.LessThan);
        private static readonly TokenListParser<YololToken, YololBinaryOp> GreaterThan = Token.EqualTo(YololToken.GreaterThan).Value(YololBinaryOp.GreaterThan);
        private static readonly TokenListParser<YololToken, YololBinaryOp> LessThanEqualTo = Token.EqualTo(YololToken.LessThanEqualTo).Value(YololBinaryOp.LessThanEqualTo);
        private static readonly TokenListParser<YololToken, YololBinaryOp> GreaterThanEqualTo = Token.EqualTo(YololToken.GreaterThanEqualTo).Value(YololBinaryOp.GreaterThanEqualTo);
        private static readonly TokenListParser<YololToken, YololBinaryOp> NotEqualTo = Token.EqualTo(YololToken.NotEqualTo).Value(YololBinaryOp.NotEqualTo);
        private static readonly TokenListParser<YololToken, YololBinaryOp> EqualTo = Token.EqualTo(YololToken.EqualTo).Value(YololBinaryOp.EqualTo);

        private static readonly TokenListParser<YololToken, VariableName> VariableName = Token.EqualTo(YololToken.Identifier).Select(n => new VariableName(n.ToStringValue()));
        private static readonly TokenListParser<YololToken, VariableName> ExternalVariableName = Token.EqualTo(YololToken.ExternalIdentifier).Select(n => new VariableName(n.ToStringValue()));

        private static readonly TokenListParser<YololToken, BaseExpression> ConstantNumExpression = Token.EqualTo(YololToken.Number).Select(n => (BaseExpression)new ConstantNumber(decimal.Parse(n.ToStringValue())));
        private static readonly TokenListParser<YololToken, BaseExpression> ConstantStrExpression = Token.EqualTo(YololToken.String).Select(n => (BaseExpression)new ConstantString(n.ToStringValue().Trim('"')));
        private static readonly TokenListParser<YololToken, BaseExpression> VariableExpression = from name in VariableName select (BaseExpression)new VariableExpression(name.Name);
        private static readonly TokenListParser<YololToken, BaseExpression> ExternalVariableExpression = from name in ExternalVariableName select (BaseExpression)new VariableExpression(name.Name);

        private static readonly TokenListParser<YololToken, BaseExpression> PreIncrement =
            from inc in Token.EqualTo(YololToken.Increment)
            from var in VariableName
            select (BaseExpression)new PreIncrement(var);

        private static readonly TokenListParser<YololToken, BaseExpression> PostIncrement =
            from var in VariableName
            from inc in Token.EqualTo(YololToken.Increment)
            select (BaseExpression)new PostIncrement(var);

        private static readonly TokenListParser<YololToken, BaseExpression> PreDecrement =
            from dec in Token.EqualTo(YololToken.Decrement)
            from var in VariableName
            select (BaseExpression)new PreDecrement(var);

        private static readonly TokenListParser<YololToken, BaseExpression> PostDecrement =
            from var in VariableName
            from dec in Token.EqualTo(YololToken.Decrement)
            select (BaseExpression)new PostDecrement(var);

        private static readonly TokenListParser<YololToken, BaseExpression> Factor =
            (from lparen in Token.EqualTo(YololToken.LParen)
             from expr in Parse.Ref(() => Expression)
             from rparen in Token.EqualTo(YololToken.RParen)
             select expr)
            .Or(PostDecrement.Try())
            .Or(PreIncrement.Try())
            .Or(PostIncrement.Try())
            .Or(PreDecrement.Try())
            .Or(ConstantNumExpression.Try())
            .Or(ConstantStrExpression.Try())
            .Or(VariableExpression.Try())
            .Or(ExternalVariableExpression.Try());

        private static readonly TokenListParser<YololToken, BaseExpression> Operand =
            (from sign in Token.EqualTo(YololToken.Subtract)
             from factor in Factor
             select (BaseExpression)new NegateExpression(factor))
            .Or(Factor).Named("expression");

        private static readonly TokenListParser<YololToken, BaseExpression> Term =
            Parse.Chain(Multiply.Or(Divide), Operand, BaseExpression.MakeBinary);

        private static readonly TokenListParser<YololToken, BaseExpression> Expression =
            Parse.Chain(Add.Or(Subtract).Or(LessThan).Or(GreaterThan).Or(LessThanEqualTo).Or(GreaterThanEqualTo).Or(NotEqualTo).Or(EqualTo), Term, BaseExpression.MakeBinary);

        private static readonly TokenListParser<YololToken, BaseStatement> Assignment =
            from lhs in VariableName.Or(ExternalVariableName)
            from op in Token.EqualTo(YololToken.Assignment)
            from rhs in Expression
            select (BaseStatement)new Assignment(lhs, rhs);

        private static readonly TokenListParser<YololToken, BaseStatement> Goto =
            from @goto in Token.EqualTo(YololToken.Goto)
            from destination in Expression
            select (BaseStatement)new Goto(destination);

        private static readonly TokenListParser<YololToken, BaseStatement> If =
            from @if in Token.EqualTo(YololToken.If)
            from cond in Expression
            from then in Token.EqualTo(YololToken.Then)
            from trueBranch in Parse.Ref(() => Statements)
            from falseBranch in (Token.EqualTo(YololToken.Else).IgnoreThen(Parse.Ref(() => Statements)).OptionalOrDefault())
            from end in Token.EqualTo(YololToken.End)
            select (BaseStatement)new If(cond, trueBranch, falseBranch);

        private static readonly TokenListParser<YololToken, BaseStatement> Statement = Assignment.Or(If).Or(Goto);

        private static readonly TokenListParser<YololToken, BaseStatement[]> Statements = 
            from statement in Statement.Many()
            select statement;

        private static readonly TokenListParser<YololToken, Line> Line =
            from statement in Statements.AtEnd()
            select new Line(statement);

        public static Tokenizer<YololToken> Instance { get; } = 
            new TokenizerBuilder<YololToken>()
                .Ignore(Span.WhiteSpace)
                .Ignore(Comment.CPlusPlusStyle)

                .Match(Character.EqualTo('<'), YololToken.LessThan)
                .Match(Character.EqualTo('>'), YololToken.GreaterThan)
                .Match(Span.EqualTo("<="), YololToken.LessThanEqualTo)
                .Match(Span.EqualTo(">="), YololToken.GreaterThanEqualTo)
                .Match(Span.EqualTo("~="), YololToken.NotEqualTo)
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

                .Match(Character.EqualTo('+'), YololToken.Plus)
                .Match(Character.EqualTo('-'), YololToken.Subtract)
                .Match(Character.EqualTo('*'), YololToken.Multiply)
                .Match(Character.EqualTo('/'), YololToken.Divide)

                .Match(Numerics.Decimal, YololToken.Number)

                .Build();

        public static Result<TokenList<YololToken>> TryTokenize(string str)
        {
            return Instance.TryTokenize(str);
        }

        public static TokenListParserResult<YololToken, Line> TryParse(TokenList<YololToken> tokens)
        {
            return Line.TryParse(tokens);
        }
    }
}

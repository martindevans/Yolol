using System;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Expressions.Binary;
using Yolol.Grammar.AST.Expressions.Unary;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Grammar
{
    public static class Parser
    {
        private static readonly TokenListParser<YololToken, YololBinaryOp> Add = Token.EqualTo(YololToken.Plus).Value(YololBinaryOp.Add);
        private static readonly TokenListParser<YololToken, YololBinaryOp> Subtract = Token.EqualTo(YololToken.Subtract).Value(YololBinaryOp.Subtract);
        private static readonly TokenListParser<YololToken, YololBinaryOp> Multiply = Token.EqualTo(YololToken.Multiply).Value(YololBinaryOp.Multiply);
        private static readonly TokenListParser<YololToken, YololBinaryOp> Divide = Token.EqualTo(YololToken.Divide).Value(YololBinaryOp.Divide);
        private static readonly TokenListParser<YololToken, YololBinaryOp> Modulo = Token.EqualTo(YololToken.Modulo).Value(YololBinaryOp.Modulo);
        private static readonly TokenListParser<YololToken, YololBinaryOp> Exponent = Token.EqualTo(YololToken.Exponent).Value(YololBinaryOp.Exponent);

        private static readonly TokenListParser<YololToken, YololBinaryOp> CompoundAdd = Token.EqualTo(YololToken.CompoundPlus).Value(YololBinaryOp.Add);
        private static readonly TokenListParser<YololToken, YololBinaryOp> CompoundSubtract = Token.EqualTo(YololToken.CompoundSubtract).Value(YololBinaryOp.Subtract);
        private static readonly TokenListParser<YololToken, YololBinaryOp> CompoundMultiply = Token.EqualTo(YololToken.CompoundMultiply).Value(YololBinaryOp.Multiply);
        private static readonly TokenListParser<YololToken, YololBinaryOp> CompoundDivide = Token.EqualTo(YololToken.CompoundDivide).Value(YololBinaryOp.Divide);
        private static readonly TokenListParser<YololToken, YololBinaryOp> CompoundModulo = Token.EqualTo(YololToken.CompoundModulo).Value(YololBinaryOp.Modulo);

        private static readonly TokenListParser<YololToken, YololBinaryOp> LessThan = Token.EqualTo(YololToken.LessThan).Value(YololBinaryOp.LessThan);
        private static readonly TokenListParser<YololToken, YololBinaryOp> GreaterThan = Token.EqualTo(YololToken.GreaterThan).Value(YololBinaryOp.GreaterThan);
        private static readonly TokenListParser<YololToken, YololBinaryOp> LessThanEqualTo = Token.EqualTo(YololToken.LessThanEqualTo).Value(YololBinaryOp.LessThanEqualTo);
        private static readonly TokenListParser<YololToken, YololBinaryOp> GreaterThanEqualTo = Token.EqualTo(YololToken.GreaterThanEqualTo).Value(YololBinaryOp.GreaterThanEqualTo);
        private static readonly TokenListParser<YololToken, YololBinaryOp> NotEqualTo = Token.EqualTo(YololToken.NotEqualTo).Value(YololBinaryOp.NotEqualTo);
        private static readonly TokenListParser<YololToken, YololBinaryOp> EqualTo = Token.EqualTo(YololToken.EqualTo).Value(YololBinaryOp.EqualTo);

        private static readonly TokenListParser<YololToken, VariableName> VariableName = Token.EqualTo(YololToken.Identifier).Select(n => new VariableName(n.ToStringValue()));
        private static readonly TokenListParser<YololToken, FunctionName> FunctionName = Token.EqualTo(YololToken.Identifier).Select(n => new FunctionName(n.ToStringValue()));
        private static readonly TokenListParser<YololToken, VariableName> ExternalVariableName = Token.EqualTo(YololToken.ExternalIdentifier).Select(n => new VariableName(n.ToStringValue()));

        private static readonly TokenListParser<YololToken, BaseExpression> ConstantNumExpression = Token.EqualTo(YololToken.Number).Select(n => (BaseExpression)new ConstantNumber(decimal.Parse(n.ToStringValue())));
        private static readonly TokenListParser<YololToken, BaseExpression> ConstantStrExpression = Token.EqualTo(YololToken.String).Select(n => (BaseExpression)new ConstantString(n.ToStringValue().Trim('"')));
        private static readonly TokenListParser<YololToken, BaseExpression> VariableExpression = from name in VariableName select (BaseExpression)new Variable(name);
        private static readonly TokenListParser<YololToken, BaseExpression> ExternalVariableExpression = from name in ExternalVariableName select (BaseExpression)new Variable(name);

        private static readonly TokenListParser<YololToken, BaseExpression> PreIncrementExpr =
            from inc in Token.EqualTo(YololToken.Increment)
            from var in VariableName
            select (BaseExpression)new PreIncrement(var);

        private static readonly TokenListParser<YololToken, BaseExpression> PostIncrementExpr =
            from var in VariableName
            from inc in Token.EqualTo(YololToken.Increment)
            select (BaseExpression)new PostIncrement(var);

        private static readonly TokenListParser<YololToken, BaseExpression> PreDecrementExpr =
            from dec in Token.EqualTo(YololToken.Decrement)
            from var in VariableName
            select (BaseExpression)new PreDecrement(var);

        private static readonly TokenListParser<YololToken, BaseExpression> PostDecrementExpr =
            from var in VariableName
            from dec in Token.EqualTo(YololToken.Decrement)
            select (BaseExpression)new PostDecrement(var);

        private static readonly TokenListParser<YololToken, BaseStatement> PreIncrementStat = PreIncrementExpr.Select(a => (BaseStatement)new ExpressionWrapper(a));
        private static readonly TokenListParser<YololToken, BaseStatement> PostIncrementStat = PostIncrementExpr.Select(a => (BaseStatement)new ExpressionWrapper(a));
        private static readonly TokenListParser<YololToken, BaseStatement> PreDecrementStat = PreDecrementExpr.Select(a => (BaseStatement)new ExpressionWrapper(a));
        private static readonly TokenListParser<YololToken, BaseStatement> PostDecrementStat = PostDecrementExpr.Select(a => (BaseStatement)new ExpressionWrapper(a));

        private static readonly TokenListParser<YololToken, BaseExpression> Factor =
            (from lparen in Token.EqualTo(YololToken.LParen)
             from expr in Parse.Ref(() => Expression)
             from rparen in Token.EqualTo(YololToken.RParen)
             select (BaseExpression)new Bracketed(expr))
            .Or(PostDecrementExpr.Try())
            .Or(PreIncrementExpr.Try())
            .Or(PostIncrementExpr.Try())
            .Or(PreDecrementExpr.Try())
            .Or(ConstantNumExpression.Try())
            .Or(ConstantStrExpression.Try())
            .Or(VariableExpression.Try())
            .Or(ExternalVariableExpression.Try());

        private static readonly TokenListParser<YololToken, BaseExpression> Operand =
            (from sign in Token.EqualTo(YololToken.Subtract)
             from factor in Factor
             select (BaseExpression)new Negate(factor))
            .Or(Factor.Try())
            .Named("expression");

        private static readonly TokenListParser<YololToken, BaseExpression> Term =
            (from func in FunctionName
             from _ in Token.EqualTo(YololToken.LParen)
             from expr in Parse.Ref(() => Expression)
             from __ in Token.EqualTo(YololToken.RParen)
             select (BaseExpression)new Application(func, expr)).Try()
            .Or(
                Parse.Chain(Multiply.Or(Divide).Or(Modulo).Or(Exponent), Operand, BaseBinaryExpression.Create).Try()
            );

        private static readonly TokenListParser<YololToken, BaseExpression> Expression =
            Parse.Chain(Add.Or(Subtract).Or(LessThan).Or(GreaterThan).Or(LessThanEqualTo).Or(GreaterThanEqualTo).Or(NotEqualTo).Or(EqualTo), Term, BaseBinaryExpression.Create);

        private static readonly TokenListParser<YololToken, BaseStatement> Assignment =
            from lhs in VariableName.Or(ExternalVariableName)
            from op in Token.EqualTo(YololToken.Assignment)
            from rhs in Expression
            select (BaseStatement)new Assignment(lhs, rhs);

        private static readonly TokenListParser<YololToken, BaseStatement> CompoundAssignment =
            from lhs in VariableName.Or(ExternalVariableName)
            from op in CompoundAdd.Or(CompoundSubtract.Try()).Or(CompoundMultiply.Try()).Or(CompoundDivide.Try()).Or(CompoundModulo.Try())
            from rhs in Expression
            select (BaseStatement)new CompoundAssignment(lhs, op, rhs);

        private static readonly TokenListParser<YololToken, BaseStatement> Goto =
            from @goto in Token.EqualTo(YololToken.Goto)
            from destination in Expression
            select (BaseStatement)new Goto(destination);

        private static readonly TokenListParser<YololToken, BaseStatement> If =
            from @if in Token.EqualTo(YololToken.If)
            from cond in Expression
            from then in Token.EqualTo(YololToken.Then)
            from trueBranch in Parse.Ref(() => Statements)
            from falseBranch in (Token.EqualTo(YololToken.Else).IgnoreThen(Parse.Ref(() => Statements)).OptionalOrDefault(Array.Empty<BaseStatement>()))
            from end in Token.EqualTo(YololToken.End)
            select (BaseStatement)new If(cond, new StatementList(trueBranch), new StatementList(falseBranch));

        private static readonly TokenListParser<YololToken, BaseStatement> Statement = 
            Assignment.Try()
              .Or(CompoundAssignment.Try())
              .Or(If.Try())
              .Or(Goto.Try())
              .Or(PreIncrementStat.Try())
              .Or(PreDecrementStat.Try())
              .Or(PostIncrementStat.Try())
              .Or(PostDecrementStat.Try());

        private static readonly TokenListParser<YololToken, BaseStatement[]> Statements = 
            from statement in Statement.Many()
            select statement;

        private static readonly TokenListParser<YololToken, Line> Line =
            from statement in Statements
            from nl in Token.EqualTo(YololToken.NewLine)
            select new Line(new StatementList(statement));

        private static readonly TokenListParser<YololToken, Program> Program =
            from lines in Line.Many()
            select new Program(lines);

        public static TokenListParserResult<YololToken, Line> TryParseLine(TokenList<YololToken> tokens)
        {
            return Line.TryParse(tokens);
        }

        public static TokenListParserResult<YololToken, Program> TryParseProgram(TokenList<YololToken> tokens)
        {
            return Program.TryParse(tokens);
        }
    }
}

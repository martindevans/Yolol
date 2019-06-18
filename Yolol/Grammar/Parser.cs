using System;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Yolol.Grammar.AST.Expressions;
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

        private static readonly TokenListParser<YololToken, YololBinaryOp> CompoundAdd = Token.EqualTo(YololToken.CompoundPlus).Value(YololBinaryOp.Add);
        private static readonly TokenListParser<YololToken, YololBinaryOp> CompoundSubtract = Token.EqualTo(YololToken.CompoundSubtract).Value(YololBinaryOp.Subtract);
        private static readonly TokenListParser<YololToken, YololBinaryOp> CompoundMultiply = Token.EqualTo(YololToken.CompoundMultiply).Value(YololBinaryOp.Multiply);
        private static readonly TokenListParser<YololToken, YololBinaryOp> CompoundDivide = Token.EqualTo(YololToken.CompoundDivide).Value(YololBinaryOp.Divide);

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

        private static readonly TokenListParser<YololToken, BaseExpression> PreIncrementExpr =
            from inc in Token.EqualTo(YololToken.Increment)
            from var in VariableName
            select (BaseExpression)new PreIncrement(var);

        private static readonly TokenListParser<YololToken, BaseStatement> PreIncrementStat =
            from inc in PreIncrementExpr
            select (BaseStatement)new ExpressionWrapper(inc);

        private static readonly TokenListParser<YololToken, BaseExpression> PostIncrementExpr =
            from var in VariableName
            from inc in Token.EqualTo(YololToken.Increment)
            select (BaseExpression)new PostIncrement(var);

        private static readonly TokenListParser<YololToken, BaseStatement> PostIncrementStat =
            from inc in PostIncrementExpr
            select (BaseStatement)new ExpressionWrapper(inc);

        private static readonly TokenListParser<YololToken, BaseExpression> PreDecrementExpr =
            from dec in Token.EqualTo(YololToken.Decrement)
            from var in VariableName
            select (BaseExpression)new PreDecrement(var);

        private static readonly TokenListParser<YololToken, BaseStatement> PreDecrementStat =
            from dec in PreDecrementExpr
            select (BaseStatement)new ExpressionWrapper(dec);

        private static readonly TokenListParser<YololToken, BaseExpression> PostDecrementExpr =
            from var in VariableName
            from dec in Token.EqualTo(YololToken.Decrement)
            select (BaseExpression)new PostDecrement(var);

        private static readonly TokenListParser<YololToken, BaseStatement> PostDecrementStat =
            from dec in PostDecrementExpr
            select (BaseStatement)new ExpressionWrapper(dec);

        private static readonly TokenListParser<YololToken, BaseExpression> Factor =
            (from lparen in Token.EqualTo(YololToken.LParen)
             from expr in Parse.Ref(() => Expression)
             from rparen in Token.EqualTo(YololToken.RParen)
             select expr)
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

        private static readonly TokenListParser<YololToken, BaseStatement> CompoundAssignment =
            from lhs in VariableName.Or(ExternalVariableName)
            from op in CompoundAdd.Or(CompoundSubtract).Or(CompoundMultiply).Or(CompoundDivide)
            from rhs in Expression
            select (BaseStatement)new Assignment(lhs, BaseExpression.MakeBinary(op, new VariableExpression(lhs.Name), rhs));

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
            select (BaseStatement)new If(cond, trueBranch, falseBranch);

        private static readonly TokenListParser<YololToken, BaseStatement> Statement = 
            Assignment.Try()
              .Or(CompoundAssignment.Try())
              .Or(If.Try())
              .Or(Goto.Try())
              .Or(PreIncrementStat.Try())
              .Or(PreDecrementStat.Try())
              .Or(PostIncrementStat.Try())
              .Or(PostDecrementStat);

        private static readonly TokenListParser<YololToken, BaseStatement[]> Statements = 
            from statement in Statement.Many()
            select statement;

        private static readonly TokenListParser<YololToken, Line> Line =
            from statement in Statements.AtEnd()
            select new Line(statement);

        public static TokenListParserResult<YololToken, Line> TryParse(TokenList<YololToken> tokens)
        {
            return Line.TryParse(tokens);
        }
    }
}

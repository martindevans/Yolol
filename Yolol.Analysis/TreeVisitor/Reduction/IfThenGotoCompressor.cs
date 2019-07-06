using System.Linq;
using JetBrains.Annotations;
using Yolol.Execution.Extensions;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Expressions.Binary;
using Yolol.Grammar.AST.Expressions.Unary;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.TreeVisitor.Reduction
{
    public class IfThenGotoCompressor
        : BaseTreeVisitor
    {
        private int _lineNumber;

        protected override Line Visit(Line line)
        {
            _lineNumber++;

            if (line.Statements.Statements.Count == 0)
                return base.Visit(line);

            var last = line.Statements.Statements.Last();
            if (!(last is If @if))
                return base.Visit(line);

            return base.Visit(new Line(new StatementList(line.Statements.Statements.Take(line.Statements.Statements.Count - 1).Append(Replace(@if)))));
        }

        [NotNull] private BaseStatement Replace([NotNull] If @if)
        {
            if (@if.FalseBranch.Statements.Count != 0)
                return @if;

            if (@if.TrueBranch.Statements.Count != 1)
                return @if;

            if (!(@if.TrueBranch.Statements.Single() is Goto @goto))
                return @if;

            // Replace:
            //      `if A then goto B end <fallthrough to next line>`
            // With:
            //      `goto B + (Next_Line * (A != 0))`

            var condition = @if.Condition.IsBoolean
                ? Invert(@if.Condition)
                : new Bracketed(new EqualTo(new Bracketed(@if.Condition), new ConstantNumber(0)));

            BaseExpression diff = new Bracketed(new Subtract(new ConstantNumber(_lineNumber + 1), @goto.Destination));
            if (diff.IsConstant)
                diff = new ConstantNumber(diff.StaticEvaluate().Number);

            var dest2 = new Add(@goto.Destination, new Multiply(diff, new Bracketed(condition)));

            return new Goto(dest2);
        }

        [NotNull] private static BaseExpression Invert([NotNull] BaseExpression expression)
        {
            switch (expression)
            {
                case EqualTo a:            return new NotEqualTo(a.Left, a.Right);
                case GreaterThan a:        return new LessThanEqualTo(a.Left, a.Right);
                case GreaterThanEqualTo a: return new LessThan(a.Left, a.Right);
                case LessThan a:           return new GreaterThanEqualTo(a.Left, a.Right);
                case LessThanEqualTo a:    return new GreaterThan(a.Left, a.Right);
                case NotEqualTo a:         return new EqualTo(a.Left, a.Right);

                default:
                    return new EqualTo(new ConstantNumber(0), new Bracketed(expression));
            }
        }
    }
}

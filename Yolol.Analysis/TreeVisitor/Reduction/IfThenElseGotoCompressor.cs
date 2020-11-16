using System.Linq;
using Yolol.Execution;
using Yolol.Execution.Extensions;
using Yolol.Grammar.AST;
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

        public override Line Visit(Line line)
        {
            _lineNumber++;

            if (line.Statements.Statements.Count == 0)
                return base.Visit(line);

            var last = line.Statements.Statements.Last();
            if (!(last is If @if))
                return base.Visit(line);

            return base.Visit(new Line(new StatementList(line.Statements.Statements.Take(line.Statements.Statements.Count - 1).Append(Replace(@if)))));
        }

        private BaseStatement Replace(If @if)
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
                : new Bracketed(new EqualTo(new Bracketed(@if.Condition), new ConstantNumber((Number)0)));

            BaseExpression diff = new Bracketed(new Subtract(new ConstantNumber((Number)_lineNumber + (Number)1), @goto.Destination));
            if (diff.IsConstant)
                diff = new ConstantNumber(diff.StaticEvaluate().Number);

            var dest2 = new Add(@goto.Destination, new Multiply(diff, new Bracketed(condition)));

            return new Goto(dest2);
        }

        private static BaseExpression Invert(BaseExpression expression)
        {
            return expression switch {
                EqualTo a => (BaseExpression)new NotEqualTo(a.Left, a.Right),
                GreaterThan a => new LessThanEqualTo(a.Left, a.Right),
                GreaterThanEqualTo a => new LessThan(a.Left, a.Right),
                LessThan a => new GreaterThanEqualTo(a.Left, a.Right),
                LessThanEqualTo a => new GreaterThan(a.Left, a.Right),
                NotEqualTo a => new EqualTo(a.Left, a.Right),
                _ => new EqualTo(new ConstantNumber((Number)0), new Bracketed(expression))
            };
        }
    }
}

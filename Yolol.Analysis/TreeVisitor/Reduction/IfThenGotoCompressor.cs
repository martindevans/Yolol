using System.Linq;
using Yolol.Grammar;
using Yolol.Grammar.AST.Expressions.Binary;
using Yolol.Grammar.AST.Expressions.Unary;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.TreeVisitor.Reduction
{
    /// <summary>
    /// Replace:
    ///     `if A then goto B else goto C end`
    /// With:
    ///     goto A*(B-C)+C
    /// Or:
    ///     x=C goto (A)*((B)-x)+x
    /// Whichever alternative is shorter
    /// </summary>
    public class IfThenElseGotoCompressor
        : BaseTreeVisitor
    {
        private readonly INameGenerator _names;

        public IfThenElseGotoCompressor(INameGenerator names)
        {
            _names = names;
        }

        protected override BaseStatement Visit(If @if)
        {
            if (@if.FalseBranch.Statements.Count != 1)
                return @if;

            if (@if.TrueBranch.Statements.Count != 1)
                return @if;

            if (!(@if.TrueBranch.Statements.Single() is Goto gotoTrue))
                return @if;

            if (!(@if.FalseBranch.Statements.Single() is Goto gotoFalse))
                return @if;

            // if A then goto B else goto C end
            var a = new Bracketed(new Or(@if.Condition, new ConstantNumber(0)));
            var b = new Bracketed(gotoTrue.Destination);
            var c = new Bracketed(gotoFalse.Destination);

            // goto A*(B-C)+C
            var g1 = new Goto(new Add(c, new Multiply(a, new Bracketed(new Subtract(b, c)))));

            // x=C goto A*(B-x)+x
            var x = new VariableName(_names.Name());
            var g2 = new StatementList(
                new Assignment(x, c),
                new Goto(new Add(new Multiply(a, new Bracketed(new Subtract(b, new Variable(x)))), new Variable(x)))
            );

            // Return the shortest one (ignoring the length of the temporary variable name, assume that's optimised to 1 char)
            var xl = x.Name.Length * 2;
            if (g1.ToString().Length <= g2.ToString().Length - xl + 2)
                return g1;
            else
                return g2;
        }
    }
}

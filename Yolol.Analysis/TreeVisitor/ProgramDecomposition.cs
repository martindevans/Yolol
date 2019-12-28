using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Yolol.Analysis.ControlFlowGraph.AST;
using Yolol.Execution.Extensions;
using Yolol.Grammar;
using Yolol.Grammar.AST;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Expressions.Binary;
using Yolol.Grammar.AST.Expressions.Unary;
using Yolol.Grammar.AST.Statements;
using Variable = Yolol.Grammar.AST.Expressions.Variable;

namespace Yolol.Analysis.TreeVisitor
{
    public class ProgramDecomposition
    {
        private readonly INameGenerator _names;

        public ProgramDecomposition(INameGenerator names)
        {
            _names = names;
        }

        [NotNull]
        public Program Visit([NotNull] Program program)
        {
            return new Program(program.Lines.Select(DecomposeLine));
        }

        [NotNull] private Line DecomposeLine([NotNull] Line line)
        {
            return new Line(new StatementList(line.Statements.Statements.SelectMany(a => new StatementDecomposition(_names).Visit(a))));
        }
    }

    /// <summary>
    /// Break a statement down into smaller statements that each do exactly one thing
    /// </summary>
    public class StatementDecomposition
        : BaseStatementVisitor<IEnumerable<BaseStatement>>
    {
        private readonly INameGenerator _names;

        public StatementDecomposition(INameGenerator names)
        {
            _names = names;
        }

        protected override IEnumerable<BaseStatement> Visit(ErrorStatement err)
        {
            yield return err;
        }

        protected override IEnumerable<BaseStatement> Visit(Conditional con)
        {
            throw new NotSupportedException();
        }

        protected override IEnumerable<BaseStatement> Visit(EmptyStatement empty)
        {
            return Array.Empty<BaseStatement>();
        }

        protected override IEnumerable<BaseStatement> Visit(StatementList list)
        {
            return list.Statements.SelectMany(Visit);
        }

        protected override IEnumerable<BaseStatement> Visit(CompoundAssignment compAss)
        {
            var tmp = new VariableName(_names.Name());

            var a = new ExpressionDecomposition(_names).Visit(compAss.Expression).ToArray();
            var b = new Assignment(tmp, BaseBinaryExpression.Create(compAss.Op, new Variable(new VariableName(compAss.Left.Name)), new Variable(((Assignment)a.Last()).Left)));
            var c = new Assignment(new VariableName(compAss.Left.Name), new Variable(tmp));

            return a.Append(b).Append(c);
        }

        protected override IEnumerable<BaseStatement> Visit(TypedAssignment ass)
        {
            throw new NotSupportedException();
        }

        protected override IEnumerable<BaseStatement> Visit(Assignment ass)
        {
            var a = new ExpressionDecomposition(_names).Visit(ass.Right).ToArray();
            var b = new Assignment(ass.Left, new Variable(((Assignment)a.Last()).Left));

            return a.Append(b);
        }

        protected override IEnumerable<BaseStatement> Visit(ExpressionWrapper expr)
        {
            return new ExpressionDecomposition(_names).Visit(expr.Expression);
        }

        protected override IEnumerable<BaseStatement> Visit(Goto @goto)
        {
            if (@goto.Destination.IsConstant)
            {
                var v = @goto.Destination.StaticEvaluate();
                if (v.Type == Execution.Type.Number)
                    return new[] { @goto };
            }

            var a = new ExpressionDecomposition(_names).Visit(@goto.Destination);
            var b = new Goto(new Variable(((Assignment)a.Last()).Left));
            return a.Append(b);
        }

        protected override IEnumerable<BaseStatement> Visit(If @if)
        {
            var a = new ExpressionDecomposition(_names).Visit(@if.Condition);
            var b = new If(new Variable(((Assignment)a.Last()).Left), new StatementList(Visit(@if.TrueBranch)), new StatementList(Visit(@if.FalseBranch)));

            return a.Append(b);
        }
    }

    /// <summary>
    /// Break a single expression down into statements that each do exactly one thing
    /// </summary>
    public class ExpressionDecomposition
        : BaseExpressionVisitor<IEnumerable<BaseStatement>>
    {
        private readonly INameGenerator _names;

        public ExpressionDecomposition(INameGenerator names)
        {
            _names = names;
        }

        [NotNull] private VariableName MkTmp() => new VariableName(_names.Name());

        [NotNull] private static Variable GetResultName([NotNull] IEnumerable<BaseStatement> stmts, BaseExpression original)
        {
            if (stmts.Any())
                return new Variable(((Assignment)stmts.Last()).Left);

            if (original is Variable var)
                return var;

            throw new InvalidOperationException("Failed to get result from previous decomposed expression");
        }

        [NotNull] private IEnumerable<BaseStatement> Binary<T>([NotNull] T expr, [NotNull] Func<BaseExpression, BaseExpression, T> factory)
            where T : BaseBinaryExpression
        {
            (BaseExpression, IEnumerable<BaseStatement>) EvalSide(BaseExpression ex)
            {
                if (ex.IsConstant)
                    return (ex.StaticEvaluate().ToConstant(), Array.Empty<BaseStatement>());

                var s = Visit(ex);
                return (GetResultName(s, ex), s);
            }

            var (le, ls) = EvalSide(expr.Left);
            var (re, rs) = EvalSide(expr.Right);

            var t = MkTmp();
            var a = new Assignment(t, factory(le, re));

            return ls.Concat(rs).Append(a);
        }

        [NotNull] private IEnumerable<BaseStatement> Unary<T>([NotNull] T _, [NotNull] BaseExpression param, [NotNull] Func<Variable, T> factory)
            where T : BaseExpression
        {
            var p = Visit(param);
            var a = new Assignment(MkTmp(), factory(GetResultName(p, param)));

            return p.Append(a);
        }

        protected override IEnumerable<BaseStatement> Visit(ErrorExpression err)
        {
            yield return new ErrorStatement();
        }

        protected override IEnumerable<BaseStatement> Visit(Increment inc)
        {
            throw new NotSupportedException();
        }

        protected override IEnumerable<BaseStatement> Visit(Decrement dec)
        {
            throw new NotSupportedException();
        }

        protected override IEnumerable<BaseStatement> Visit(Phi phi)
        {
            throw new NotSupportedException();
        }

        protected override IEnumerable<BaseStatement> Visit(LessThanEqualTo eq) => Binary(eq, (a, b) => new LessThanEqualTo(a, b));

        protected override IEnumerable<BaseStatement> Visit(LessThan eq) => Binary(eq, (a, b) => new LessThan(a, b));

        protected override IEnumerable<BaseStatement> Visit(GreaterThanEqualTo eq) => Binary(eq, (a, b) => new GreaterThanEqualTo(a, b));

        protected override IEnumerable<BaseStatement> Visit(GreaterThan eq) => Binary(eq, (a, b) => new GreaterThan(a, b));

        protected override IEnumerable<BaseStatement> Visit(NotEqualTo eq) => Binary(eq, (a, b) => new NotEqualTo(a, b));

        protected override IEnumerable<BaseStatement> Visit(EqualTo eq) => Binary(eq, (a, b) => new EqualTo(a, b));

        protected override IEnumerable<BaseStatement> Visit(Variable var)
        {
            return new[] { new Assignment(var.Name, var) };
        }

        protected override IEnumerable<BaseStatement> Visit(Modulo mod) => Binary(mod, (a, b) => new Modulo(a, b));

        protected override IEnumerable<BaseStatement> Visit(PreDecrement dec)
        {
            return new BaseStatement[] {

                // Increment in place
                new Assignment(dec.Name, new Decrement(dec.Name)),

                // Return modified value
                new Assignment(MkTmp(), new Variable(dec.Name))
            };
        }

        protected override IEnumerable<BaseStatement> Visit(PostDecrement dec)
        {
            var tmp = MkTmp();
            return new BaseStatement[] {

                // Save original value
                new Assignment(tmp, new Variable(dec.Name)),

                // Increment it in place
                new Assignment(dec.Name, new Decrement(dec.Name)),

                // Return original value
                new Assignment(MkTmp(), new Variable(tmp))
            };
        }

        protected override IEnumerable<BaseStatement> Visit(PreIncrement inc)
        {
            return new BaseStatement[] {

                // Increment in place
                new Assignment(inc.Name, new Increment(inc.Name)),

                // Return modified value
                new Assignment(MkTmp(), new Variable(inc.Name))
            };
        }

        protected override IEnumerable<BaseStatement> Visit(PostIncrement inc)
        {
            var tmp = MkTmp();
            return new BaseStatement[] {

                // Save original value
                new Assignment(tmp, new Variable(inc.Name)),

                // Increment it in place
                new Assignment(inc.Name, new Increment(inc.Name)),

                // Return original value
                new Assignment(MkTmp(), new Variable(tmp))
            };
        }

        protected override IEnumerable<BaseStatement> Visit(Abs app) => Unary(app, app.Parameter, a => new Abs(a));

        protected override IEnumerable<BaseStatement> Visit(Sqrt app) => Unary(app, app.Parameter, a => new Sqrt(a));

        protected override IEnumerable<BaseStatement> Visit(Sine app) => Unary(app, app.Parameter, a => new Sine(a));

        protected override IEnumerable<BaseStatement> Visit(Cosine app) => Unary(app, app.Parameter, a => new Cosine(a));

        protected override IEnumerable<BaseStatement> Visit(Tangent app) => Unary(app, app.Parameter, a => new Tangent(a));

        protected override IEnumerable<BaseStatement> Visit(ArcSine app) => Unary(app, app.Parameter, a => new ArcSine(a));

        protected override IEnumerable<BaseStatement> Visit(ArcCos app) => Unary(app, app.Parameter, a => new ArcCos(a));

        protected override IEnumerable<BaseStatement> Visit(ArcTan app) => Unary(app, app.Parameter, a => new ArcTan(a));

        protected override IEnumerable<BaseStatement> Visit(Bracketed brk) => Visit(brk.Parameter);

        protected override IEnumerable<BaseStatement> Visit(And and) => Binary(and, (a, b) => new And(a, b));

        protected override IEnumerable<BaseStatement> Visit(Or or) => Binary(or, (a, b) => new Or(a, b));

        protected override IEnumerable<BaseStatement> Visit(Not not) => Unary(not, not.Parameter, a => new Not(a));

        protected override IEnumerable<BaseStatement> Visit(Add add) => Binary(add, (a, b) => new Add(a, b));

        protected override IEnumerable<BaseStatement> Visit(Subtract sub) => Binary(sub, (a, b) => new Subtract(a, b));

        protected override IEnumerable<BaseStatement> Visit(Multiply mul) => Binary(mul, (a, b) => new Multiply(a, b));

        protected override IEnumerable<BaseStatement> Visit(Divide div) => Binary(div, (a, b) => new Divide(a, b));

        protected override IEnumerable<BaseStatement> Visit(Exponent exp) => Binary(exp, (a, b) => new Exponent(a, b));

        protected override IEnumerable<BaseStatement> Visit(Negate neg) => Unary(neg, neg.Parameter, a => new Negate(a));

        protected override IEnumerable<BaseStatement> Visit(ConstantNumber con) => new[] { new Assignment(MkTmp(), con) };

        protected override IEnumerable<BaseStatement> Visit(ConstantString con) => new[] { new Assignment(MkTmp(), con) };
    }
}

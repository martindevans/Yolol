using System;
using System.Linq;
using Yolol.Analysis.ControlFlowGraph.AST;
using Yolol.Grammar;
using Yolol.Grammar.AST;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Expressions.Binary;
using Yolol.Grammar.AST.Expressions.Unary;
using Yolol.Grammar.AST.Statements;
using Variable = Yolol.Grammar.AST.Expressions.Variable;

namespace Yolol.Analysis.TreeVisitor
{
    public abstract class BaseTreeVisitor
        : ITreeVisitor
    {
        public virtual Program Visit(Program program)
        {
            return new Program(program.Lines.Select(Visit));
        }

        public virtual Line Visit(Line line)
        {
            return new Line(Visit(line.Statements));
        }

        protected virtual VariableName Visit(VariableName var)
        {
            return var;
        }

        #region expression visiting
        public virtual BaseExpression Visit(BaseExpression expression)
        {
            return expression switch {
                Phi a => Visit(a),
                Increment a => Visit(a),
                Decrement a => Visit(a),
                ErrorExpression a => Visit(a),
                Bracketed a => Visit(a),
                Abs a => Visit(a),
                Sqrt a => Visit(a),
                Sine a => Visit(a),
                Cosine a => Visit(a),
                Tangent a => Visit(a),
                ArcSine a => Visit(a),
                ArcCos a => Visit(a),
                ArcTan a => Visit(a),
                PostIncrement a => Visit(a),
                PreIncrement a => Visit(a),
                PostDecrement a => Visit(a),
                PreDecrement a => Visit(a),
                Add a => Visit(a),
                Subtract a => Visit(a),
                Multiply a => Visit(a),
                Divide a => Visit(a),
                Modulo a => Visit(a),
                Negate a => Visit(a),
                Exponent a => Visit(a),
                Or a => Visit(a),
                And a => Visit(a),
                Not a => Visit(a),
                Variable a => Visit(a),
                ConstantNumber a => Visit(a),
                ConstantString a => Visit(a),
                EqualTo a => Visit(a),
                NotEqualTo a => Visit(a),
                GreaterThan a => Visit(a),
                GreaterThanEqualTo a => Visit(a),
                LessThan a => Visit(a),
                LessThanEqualTo a => Visit(a),
                Factorial a => Visit(a),
                _ => VisitUnknown(expression)
            };
        }

        protected virtual BaseExpression VisitUnknown(BaseExpression expression)
        {
            throw new InvalidOperationException($"`Visit` not valid for expression type `{expression.GetType().FullName}`");
        }

        protected virtual BaseExpression Visit(Or or)
        {
            return new Or(Visit(or.Left), Visit(or.Right));
        }

        protected virtual BaseExpression Visit(And and)
        {
            return new And(Visit(and.Left), Visit(and.Right));
        }

        protected virtual BaseExpression Visit(Not not)
        {
            return new Not(Visit(not.Parameter));
        }

        protected virtual BaseExpression Visit(ErrorExpression err)
        {
            return err;
        }

        protected virtual BaseExpression Visit(Increment inc)
        {
            var v = new Variable(inc.Name);
            var r = (Variable)Visit(v);

            return new Increment(r.Name);
        }

        protected virtual BaseExpression Visit(Decrement dec)
        {
            var v = Visit(new Variable(dec.Name));
            var r = (Variable)v;

            return new Decrement(r.Name);
        }

        protected virtual BaseExpression Visit(Phi phi)
        {
            return new Phi(phi.SSA, phi.AssignedNames.Select(Visit).ToArray());
        }

        protected virtual BaseExpression Visit(Factorial fac)
        {
            return new Factorial(Visit(fac.Parameter));
        }

        protected virtual BaseExpression Visit(LessThanEqualTo eq)
        {
            return new LessThanEqualTo(Visit(eq.Left), Visit(eq.Right));
        }

        protected virtual BaseExpression Visit(LessThan eq)
        {
            return new LessThan(Visit(eq.Left), Visit(eq.Right));
        }

        protected virtual BaseExpression Visit(GreaterThanEqualTo eq)
        {
            return new GreaterThanEqualTo(Visit(eq.Left), Visit(eq.Right));
        }

        protected virtual BaseExpression Visit(GreaterThan eq)
        {
            return new GreaterThan(Visit(eq.Left), Visit(eq.Right));
        }

        protected virtual BaseExpression Visit(NotEqualTo eq)
        {
            return new NotEqualTo(Visit(eq.Left), Visit(eq.Right));
        }

        protected virtual BaseExpression Visit(EqualTo eq)
        {
            return new EqualTo(Visit(eq.Left), Visit(eq.Right));
        }

        protected virtual BaseExpression Visit(Variable var)
        {
            return new Variable(Visit(var.Name));
        }

        protected virtual BaseExpression Visit(Modulo mod)
        {
            return new Modulo(Visit(mod.Left), Visit(mod.Right));
        }

        protected virtual BaseExpression Visit(PreDecrement dec)
        {
            return new PreDecrement(Visit(dec.Name));
        }

        protected virtual BaseExpression Visit(PostDecrement dec)
        {
            return new PostDecrement(Visit(dec.Name));
        }

        protected virtual BaseExpression Visit(PreIncrement inc)
        {
            return new PreIncrement(Visit(inc.Name));
        }

        protected virtual BaseExpression Visit(PostIncrement inc)
        {
            return new PostIncrement(Visit(inc.Name));
        }

        protected virtual BaseExpression Visit(Abs app)
        {
            return new Abs(Visit(app.Parameter));
        }

        protected virtual BaseExpression Visit(Sqrt app)
        {
            return new Sqrt(Visit(app.Parameter));
        }

        protected virtual BaseExpression Visit(Sine app)
        {
            return new Sine(Visit(app.Parameter));
        }

        protected virtual BaseExpression Visit(Cosine app)
        {
            return new Cosine(Visit(app.Parameter));
        }

        protected virtual BaseExpression Visit(Tangent app)
        {
            return new Tangent(Visit(app.Parameter));
        }

        protected virtual BaseExpression Visit(ArcSine app)
        {
            return new ArcSine(Visit(app.Parameter));
        }

        protected virtual BaseExpression Visit(ArcCos app)
        {
            return new ArcCos(Visit(app.Parameter));
        }

        protected virtual BaseExpression Visit(ArcTan app)
        {
            return new ArcTan(Visit(app.Parameter));
        }

        protected virtual BaseExpression Visit(Bracketed brk)
        {
            return new Bracketed(Visit(brk.Parameter));
        }

        protected virtual BaseExpression Visit(Add add)
        {
            return new Add(Visit(add.Left), Visit(add.Right));
        }

        protected virtual BaseExpression Visit(Subtract sub)
        {
            return new Subtract(Visit(sub.Left), Visit(sub.Right));
        }

        protected virtual BaseExpression Visit(Multiply mul)
        {
            return new Multiply(Visit(mul.Left), Visit(mul.Right));
        }

        protected virtual BaseExpression Visit(Divide div)
        {
            return new Divide(Visit(div.Left), Visit(div.Right));
        }

        protected virtual BaseExpression Visit(Exponent exp)
        {
            return new Exponent(Visit(exp.Left), Visit(exp.Right));
        }

        protected virtual BaseExpression Visit(Negate neg)
        {
            return new Negate(Visit(neg.Parameter));
        }

        protected virtual BaseExpression Visit(ConstantNumber con)
        {
            return con;
        }

        protected virtual BaseExpression Visit(ConstantString con)
        {
            return con;
        }
        #endregion

        #region statement visiting
        public BaseStatement Visit(BaseStatement statement)
        {
            return statement switch {
                Conditional a => Visit(a),
                TypedAssignment a => Visit(a),
                ErrorStatement a => Visit(a),
                CompoundAssignment a => Visit(a),
                Assignment a => Visit(a),
                ExpressionWrapper a => Visit(a),
                Goto a => Visit(a),
                If a => Visit(a),
                StatementList a => Visit(a),
                EmptyStatement a => a,
                _ => VisitUnknown(statement)
            };
        }

        protected virtual BaseStatement VisitUnknown(BaseStatement statement)
        {
            throw new InvalidOperationException($"`Visit` invalid for statement type `{statement.GetType().FullName}`");
        }

        protected virtual BaseStatement Visit(ErrorStatement err)
        {
            return err;
        }

        protected virtual BaseStatement Visit(Conditional con)
        {
            return new Conditional(Visit(con.Condition));
        }

        protected virtual BaseStatement Visit(TypedAssignment ass)
        {
            var expr = Visit(ass.Right);
            return new TypedAssignment(ass.Type, Visit(ass.Left), expr);
        }

        protected virtual StatementList Visit(StatementList list)
        {
            return new StatementList(list.Statements.Select(Visit));
        }

        protected virtual BaseStatement Visit(CompoundAssignment compAss)
        {
            var expr = Visit(compAss.Expression);
            return new CompoundAssignment(Visit(compAss.Left), compAss.Op, expr);
        }

        protected virtual BaseStatement Visit(Assignment ass)
        {
            var expr = Visit(ass.Right);
            return new Assignment(Visit(ass.Left), expr);
        }

        protected virtual BaseStatement Visit(ExpressionWrapper expr)
        {
            return new ExpressionWrapper(Visit(expr.Expression));
        }

        protected virtual BaseStatement Visit(Goto @goto)
        {
            return new Goto(Visit(@goto.Destination));
        }

        protected virtual BaseStatement Visit(If @if)
        {
            return new If(Visit(@if.Condition), Visit(@if.TrueBranch), Visit(@if.FalseBranch));
        }
        #endregion
    }
}

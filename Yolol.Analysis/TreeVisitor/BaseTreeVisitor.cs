using System;
using System.Linq;
using JetBrains.Annotations;
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

        [NotNull] public virtual Line Visit([NotNull] Line line)
        {
            var r = Visit(line.Statements);
            if (r is StatementList sl)
                return new Line(sl);
            return new Line(new StatementList(r));
        }

        [NotNull] protected virtual VariableName Visit([NotNull] VariableName var)
        {
            return var;
        }

        #region expression visiting
        [NotNull] public virtual BaseExpression Visit([NotNull] BaseExpression expression)
        {
            switch (expression)
            {
                case Phi a:         return Visit(a);
                case Increment a:   return Visit(a);
                case Decrement a:   return Visit(a);
                case ErrorExpression a: return Visit(a);

                case Bracketed a:   return Visit(a);

                case Abs a:     return Visit(a);
                case Sqrt a:    return Visit(a);
                case Sine a:    return Visit(a);
                case Cosine a:  return Visit(a);
                case Tangent a: return Visit(a);
                case ArcSine a: return Visit(a);
                case ArcCos a:  return Visit(a);
                case ArcTan a:  return Visit(a);

                case PostIncrement a: return Visit(a);
                case PreIncrement a:  return Visit(a);
                case PostDecrement a: return Visit(a);
                case PreDecrement a:  return Visit(a);

                case Add a:      return Visit(a);
                case Subtract a: return Visit(a);
                case Multiply a: return Visit(a);
                case Divide a:   return Visit(a);
                case Modulo a:   return Visit(a);
                case Negate a:   return Visit(a);
                case Exponent a: return Visit(a);

                case Or a:  return Visit(a);
                case And a: return Visit(a);
                case Not a: return Visit(a);

                case Variable a:       return Visit(a);
                case ConstantNumber a: return Visit(a);
                case ConstantString a: return Visit(a);

                case EqualTo a:            return Visit(a);
                case NotEqualTo a:         return Visit(a);
                case GreaterThan a:        return Visit(a);
                case GreaterThanEqualTo a: return Visit(a);
                case LessThan a:           return Visit(a);
                case LessThanEqualTo a:    return Visit(a);
            }

            return VisitUnknown(expression);
        }

        [NotNull] protected virtual BaseExpression VisitUnknown(BaseExpression expression)
        {
            throw new InvalidOperationException($"`Visit` not invalid for expression type `{expression.GetType().FullName}`");
        }

        [NotNull] protected virtual BaseExpression Visit([NotNull] Or or)
        {
            return new Or(Visit(or.Left), Visit(or.Right));
        }

        [NotNull] protected virtual BaseExpression Visit([NotNull] And and)
        {
            return new And(Visit(and.Left), Visit(and.Right));
        }

        [NotNull]
        protected virtual BaseExpression Visit([NotNull] Not not)
        {
            return new Not(Visit(not.Parameter));
        }

        [NotNull] protected virtual BaseExpression Visit([NotNull] ErrorExpression err)
        {
            return err;
        }

        [NotNull] protected virtual BaseExpression Visit([NotNull] Increment inc)
        {
            var v = new Variable(inc.Name);
            var r = (Variable)Visit(v);

            return new Increment(r.Name);
        }

        [NotNull] protected virtual BaseExpression Visit([NotNull] Decrement dec)
        {
            var v = Visit(new Variable(dec.Name));
            var r = (Variable)v;

            return new Decrement(r.Name);
        }

        [NotNull] protected virtual BaseExpression Visit([NotNull] Phi phi)
        {
            return new Phi(phi.SSA, phi.AssignedNames.Select(Visit).ToArray());
        }

        [NotNull] protected virtual BaseExpression Visit([NotNull] LessThanEqualTo eq)
        {
            return new LessThanEqualTo(Visit(eq.Left), Visit(eq.Right));
        }

        [NotNull] protected virtual BaseExpression Visit([NotNull] LessThan eq)
        {
            return new LessThan(Visit(eq.Left), Visit(eq.Right));
        }

        [NotNull] protected virtual BaseExpression Visit([NotNull] GreaterThanEqualTo eq)
        {
            return new GreaterThanEqualTo(Visit(eq.Left), Visit(eq.Right));
        }

        [NotNull] protected virtual BaseExpression Visit([NotNull] GreaterThan eq)
        {
            return new GreaterThan(Visit(eq.Left), Visit(eq.Right));
        }

        [NotNull] protected virtual BaseExpression Visit([NotNull] NotEqualTo eq)
        {
            return new NotEqualTo(Visit(eq.Left), Visit(eq.Right));
        }

        [NotNull] protected virtual BaseExpression Visit([NotNull] EqualTo eq)
        {
            return new EqualTo(Visit(eq.Left), Visit(eq.Right));
        }

        [NotNull] protected virtual BaseExpression Visit([NotNull] Variable var)
        {
            return new Variable(Visit(var.Name));
        }

        [NotNull] protected virtual BaseExpression Visit([NotNull] Modulo mod)
        {
            return new Modulo(Visit(mod.Left), Visit(mod.Right));
        }

        [NotNull] protected virtual BaseExpression Visit([NotNull] PreDecrement dec)
        {
            return new PreDecrement(Visit(dec.Name));
        }

        [NotNull] protected virtual BaseExpression Visit([NotNull] PostDecrement dec)
        {
            return new PostDecrement(Visit(dec.Name));
        }

        [NotNull] protected virtual BaseExpression Visit([NotNull] PreIncrement inc)
        {
            return new PreIncrement(Visit(inc.Name));
        }

        [NotNull] protected virtual BaseExpression Visit([NotNull] PostIncrement inc)
        {
            return new PostIncrement(Visit(inc.Name));
        }

        [NotNull] protected virtual BaseExpression Visit([NotNull] Abs app)
        {
            return new Abs(Visit(app.Parameter));
        }

        [NotNull] protected virtual BaseExpression Visit([NotNull] Sqrt app)
        {
            return new Sqrt(Visit(app.Parameter));
        }

        [NotNull] protected virtual BaseExpression Visit([NotNull] Sine app)
        {
            return new Sine(Visit(app.Parameter));
        }

        [NotNull] protected virtual BaseExpression Visit([NotNull] Cosine app)
        {
            return new Cosine(Visit(app.Parameter));
        }

        [NotNull] protected virtual BaseExpression Visit([NotNull] Tangent app)
        {
            return new Tangent(Visit(app.Parameter));
        }

        [NotNull] protected virtual BaseExpression Visit([NotNull] ArcSine app)
        {
            return new ArcSine(Visit(app.Parameter));
        }

        [NotNull] protected virtual BaseExpression Visit([NotNull] ArcCos app)
        {
            return new ArcCos(Visit(app.Parameter));
        }

        [NotNull] protected virtual BaseExpression Visit([NotNull] ArcTan app)
        {
            return new ArcTan(Visit(app.Parameter));
        }

        [NotNull] protected virtual BaseExpression Visit([NotNull] Bracketed brk)
        {
            return new Bracketed(Visit(brk.Parameter));
        }

        [NotNull] protected virtual BaseExpression Visit([NotNull] Add add)
        {
            return new Add(Visit(add.Left), Visit(add.Right));
        }

        [NotNull] protected virtual BaseExpression Visit([NotNull] Subtract sub)
        {
            return new Subtract(Visit(sub.Left), Visit(sub.Right));
        }

        [NotNull] protected virtual BaseExpression Visit([NotNull] Multiply mul)
        {
            return new Multiply(Visit(mul.Left), Visit(mul.Right));
        }

        [NotNull] protected virtual BaseExpression Visit([NotNull] Divide div)
        {
            return new Divide(Visit(div.Left), Visit(div.Right));
        }

        [NotNull] protected virtual BaseExpression Visit([NotNull] Exponent exp)
        {
            return new Exponent(Visit(exp.Left), Visit(exp.Right));
        }

        [NotNull] protected virtual BaseExpression Visit([NotNull] Negate neg)
        {
            return new Negate(Visit(neg.Parameter));
        }

        [NotNull] protected virtual BaseExpression Visit([NotNull] ConstantNumber con)
        {
            return con;
        }

        [NotNull] protected virtual BaseExpression Visit([NotNull] ConstantString con)
        {
            return con;
        }
        #endregion

        #region statement visiting
        [NotNull] public BaseStatement Visit([NotNull] BaseStatement statement)
        {
            switch (statement)
            {
                case Conditional a: return Visit(a);
                case TypedAssignment a: return Visit(a);
                case ErrorStatement a: return Visit(a);

                case CompoundAssignment a: return Visit(a);
                case Assignment a: return Visit(a);
                case ExpressionWrapper a: return Visit(a);
                case Goto a: return Visit(a);
                case If a: return Visit(a);
                case StatementList a: return Visit(a);
                case EmptyStatement a: return a;
            }

            return VisitUnknown(statement);
        }

        [NotNull] protected virtual BaseStatement VisitUnknown([NotNull] BaseStatement statement)
        {
            throw new InvalidOperationException($"`Visit` invalid for statement type `{statement.GetType().FullName}`");
        }

        [NotNull] protected virtual BaseStatement Visit([NotNull] ErrorStatement err)
        {
            return err;
        }

        [NotNull] protected virtual BaseStatement Visit([NotNull] Conditional con)
        {
            return new Conditional(Visit(con.Condition));
        }

        [NotNull] protected virtual BaseStatement Visit([NotNull] TypedAssignment ass)
        {
            var expr = Visit(ass.Right);
            return new TypedAssignment(ass.Type, Visit(ass.Left), expr);
        }

        [NotNull] protected virtual StatementList Visit([NotNull] StatementList list)
        {
            return new StatementList(list.Statements.Select(Visit));
        }

        [NotNull] protected virtual BaseStatement Visit([NotNull] CompoundAssignment compAss)
        {
            var expr = Visit(compAss.Expression);
            return new CompoundAssignment(Visit(compAss.Left), compAss.Op, expr);
        }

        [NotNull] protected virtual BaseStatement Visit([NotNull] Assignment ass)
        {
            var expr = Visit(ass.Right);
            return new Assignment(Visit(ass.Left), expr);
        }

        [NotNull] protected virtual BaseStatement Visit([NotNull] ExpressionWrapper expr)
        {
            return new ExpressionWrapper(Visit(expr.Expression));
        }

        [NotNull] protected virtual BaseStatement Visit([NotNull] Goto @goto)
        {
            return new Goto(Visit(@goto.Destination));
        }

        [NotNull] protected virtual BaseStatement Visit([NotNull] If @if)
        {
            return new If(Visit(@if.Condition), Visit(@if.TrueBranch), Visit(@if.FalseBranch));
        }
        #endregion
    }
}

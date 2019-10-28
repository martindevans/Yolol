using System;
using JetBrains.Annotations;
using Microsoft.Z3;
using Yolol.Analysis.ControlFlowGraph;
using Yolol.Analysis.ControlFlowGraph.AST;
using Yolol.Analysis.TreeVisitor;
using Yolol.Analysis.Types;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Expressions.Binary;
using Yolol.Grammar.AST.Expressions.Unary;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.SAT
{
    public static class BuilderExtensions
    {
        [NotNull] public static IModel BuildSAT([NotNull] this IBasicBlock block, [NotNull] ITypeAssignments types)
        {
            var ctx = new Context();
            var solver = ctx.MkSolver();
            var model = new Model(ctx, solver);

            foreach (var stmt in block.Statements)
                Assert(model, types, stmt);

            return model;
        }

        private static void Assert([NotNull] Model model, [NotNull] ITypeAssignments types, [NotNull] BaseStatement stmt)
        {
            switch (stmt)
            {
                default:
                    throw new ArgumentOutOfRangeException(stmt.GetType().Name);

                case ExpressionWrapper _:
                case CompoundAssignment _:
                case If _:
                    throw new NotSupportedException(stmt.GetType().Name);

                case EmptyStatement _:
                    return;

                case Conditional conditional:
                    Assert(model, conditional);
                    return;

                case ErrorStatement errorStatement:
                    Assert(model, errorStatement);
                    return;

                case TypedAssignment typedAssignment:
                    Assert(model, types, typedAssignment);
                    return;

                case Assignment assignment:
                    Assert(model, types, assignment);
                    return;

                case Goto @goto:
                    Assert(model, @goto);
                    return;

                case StatementList statementList:
                    foreach (var sub in statementList.Statements)
                        Assert(model, types, sub);
                    return;
            }
        }

        private static void Assert(Model model, Goto @goto)
        {
            throw new NotImplementedException();
        }

        private static void Assert([NotNull] Model model, [NotNull] ITypeAssignments types, [NotNull] Assignment assignment)
        {
            throw new NotImplementedException();
        }

        private static void Assert([NotNull] Model model, [NotNull] ITypeAssignments types, TypedAssignment typedAssignment)
        {
            if (typedAssignment.Type.HasFlag(Execution.Type.Number))
            {
                var l = model.GetOrCreateVariable(typedAssignment.Left);
                var r = new ExpressionConstraintBuilder(model).Visit(typedAssignment.Right);

                model.Assert(model.Context.MkEq(l, r));

            }
            else
                throw new NotImplementedException();
        }

        private static void Assert(IModel model, ErrorStatement error)
        {
            throw new NotImplementedException();
        }

        private static void Assert(IModel model, Conditional conditional)
        {
            throw new NotImplementedException();
        }

        private class ExpressionConstraintBuilder
            : BaseExpressionVisitor<Expr>
        {
            private readonly Model _model;

            public ExpressionConstraintBuilder(Model model)
            {
                _model = model;
            }

            protected override Expr Visit(Or or)
            {
                throw new NotImplementedException();
            }

            protected override Expr Visit(And and)
            {
                throw new NotImplementedException();
            }

            protected override Expr Visit(ErrorExpression err)
            {
                throw new NotImplementedException();
            }

            protected override Expr Visit(Increment inc)
            {
                throw new NotImplementedException();
            }

            protected override Expr Visit(Decrement dec)
            {
                throw new NotImplementedException();
            }

            protected override Expr Visit(Phi phi)
            {
                throw new NotImplementedException();
            }

            protected override Expr Visit(LessThanEqualTo eq)
            {
                throw new NotImplementedException();
            }

            protected override Expr Visit(LessThan eq)
            {
                throw new NotImplementedException();
            }

            protected override Expr Visit(GreaterThanEqualTo eq)
            {
                throw new NotImplementedException();
            }

            protected override Expr Visit(GreaterThan eq)
            {
                throw new NotImplementedException();
            }

            protected override Expr Visit(NotEqualTo eq)
            {
                throw new NotImplementedException();
            }

            protected override Expr Visit(EqualTo eq)
            {
                throw new NotImplementedException();
            }

            protected override Expr Visit(Variable var)
            {
                return _model.GetOrCreateVariable(var.Name);
            }

            protected override Expr Visit(Modulo mod)
            {
                throw new NotImplementedException();
            }

            protected override Expr Visit(PreDecrement dec)
            {
                throw new NotImplementedException();
            }

            protected override Expr Visit(PostDecrement dec)
            {
                throw new NotImplementedException();
            }

            protected override Expr Visit(PreIncrement inc)
            {
                throw new NotImplementedException();
            }

            protected override Expr Visit(PostIncrement inc)
            {
                throw new NotImplementedException();
            }

            protected override Expr Visit(Abs app)
            {
                throw new NotImplementedException();
            }

            protected override Expr Visit(Sqrt app)
            {
                throw new NotImplementedException();
            }

            protected override Expr Visit(Sine app)
            {
                throw new NotImplementedException();
            }

            protected override Expr Visit(Cosine app)
            {
                throw new NotImplementedException();
            }

            protected override Expr Visit(Tangent app)
            {
                throw new NotImplementedException();
            }

            protected override Expr Visit(ArcSine app)
            {
                throw new NotImplementedException();
            }

            protected override Expr Visit(ArcCos app)
            {
                throw new NotImplementedException();
            }

            protected override Expr Visit(ArcTan app)
            {
                throw new NotImplementedException();
            }

            protected override Expr Visit(Bracketed brk)
            {
                throw new NotImplementedException();
            }

            protected override Expr Visit(Add add)
            {
                throw new NotImplementedException();
            }

            protected override Expr Visit(Subtract sub)
            {
                throw new NotImplementedException();
            }

            protected override Expr Visit(Multiply mul)
            {
                throw new NotImplementedException();
            }

            protected override Expr Visit(Divide div)
            {
                throw new NotImplementedException();
            }

            protected override Expr Visit(Exponent exp)
            {
                throw new NotImplementedException();
            }

            protected override Expr Visit(Negate neg)
            {
                throw new NotImplementedException();
            }

            protected override Expr Visit(ConstantNumber con)
            {
                return _model.MakeNumber(con.Value);
            }

            protected override Expr Visit(ConstantString con)
            {
                throw new NotImplementedException();
            }
        }
    }
}

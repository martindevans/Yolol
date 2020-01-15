using System;
using JetBrains.Annotations;
using Microsoft.Z3;
using Yolol.Analysis.ControlFlowGraph;
using Yolol.Analysis.ControlFlowGraph.AST;
using Yolol.Analysis.Types;
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

        private static void Assert([NotNull] Model model, [NotNull] Goto @goto)
        {
            var d = model.GetGotoVariable();
            d.AssertEq(@goto.Destination);
        }

        private static void Assert([NotNull] Model model, [NotNull] ITypeAssignments types, [NotNull] Assignment assignment)
        {
            var l = model.GetOrCreateVariable(assignment.Left);
            l.AssertEq(assignment.Right);
        }

        private static void Assert(IModel model, ErrorStatement error)
        {
            // Nothing needs asserting for an error
        }

        private static void Assert(IModel model, Conditional conditional)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using JetBrains.Annotations;
using Yolol.Analysis.ControlFlowGraph.AST;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.TreeVisitor
{
    public abstract class BaseStatementVisitor<TResult>
    {
        [NotNull] public TResult Visit([NotNull] BaseStatement statement)
        {
            switch (statement)
            {
                case Conditional a: return Visit(a);
                case TypedAssignment a: return Visit(a);
                case ErrorStatement a: return Visit(a);

                case CompoundAssignment a: return Visit(a);
                case Assignment a:   return Visit(a);
                case ExpressionWrapper a: return Visit(a);
                case Goto a: return Visit(a);
                case If a: return Visit(a);
                case StatementList a: return Visit(a);
                case EmptyStatement a: return Visit(a);
            }

            return VisitUnknown(statement);
        }

        [NotNull] protected virtual TResult VisitUnknown(BaseStatement statement)
        {
            throw new InvalidOperationException($"`Visit` invalid for statement type `{statement.GetType().FullName}`");
        }

        [NotNull] protected abstract TResult Visit([NotNull] ErrorStatement err);

        [NotNull] protected abstract TResult Visit([NotNull] Conditional con);

        [NotNull] protected abstract TResult Visit([NotNull] TypedAssignment ass);

        [NotNull] protected abstract TResult Visit([NotNull] EmptyStatement empty);

        [NotNull] protected abstract TResult Visit([NotNull] StatementList list);

        [NotNull] protected abstract TResult Visit([NotNull] CompoundAssignment compAss);

        [NotNull] protected abstract TResult Visit([NotNull] Assignment ass);

        [NotNull] protected abstract TResult Visit([NotNull] ExpressionWrapper expr);

        [NotNull] protected abstract TResult Visit([NotNull] Goto @goto);

        [NotNull] protected abstract TResult Visit([NotNull] If @if);
    }
}

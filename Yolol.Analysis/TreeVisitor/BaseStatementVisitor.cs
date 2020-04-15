using System;
using Yolol.Analysis.ControlFlowGraph.AST;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.TreeVisitor
{
    public abstract class BaseStatementVisitor<TResult>
    {
        public TResult Visit(BaseStatement statement)
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
                EmptyStatement a => Visit(a),
                _ => VisitUnknown(statement)
            };
        }

        protected virtual TResult VisitUnknown(BaseStatement statement)
        {
            throw new InvalidOperationException($"`Visit` invalid for statement type `{statement.GetType().FullName}`");
        }

        protected abstract TResult Visit(ErrorStatement err);

        protected abstract TResult Visit(Conditional con);

        protected abstract TResult Visit(TypedAssignment ass);

        protected abstract TResult Visit(EmptyStatement empty);

        protected abstract TResult Visit(StatementList list);

        protected abstract TResult Visit(CompoundAssignment compAss);

        protected abstract TResult Visit(Assignment ass);

        protected abstract TResult Visit(ExpressionWrapper expr);

        protected abstract TResult Visit(Goto @goto);

        protected abstract TResult Visit(If @if);
    }
}

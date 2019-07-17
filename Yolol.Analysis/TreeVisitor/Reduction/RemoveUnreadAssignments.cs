using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Yolol.Analysis.ControlFlowGraph.AST;
using Yolol.Analysis.ControlFlowGraph.Extensions;
using Yolol.Grammar;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.TreeVisitor.Reduction
{
    public class RemoveUnreadAssignments
        : BaseTreeVisitor
    {
        private readonly IReadOnlyCollection<VariableName> _toRemove;
        private readonly ISingleStaticAssignmentTable _ssa;

        public RemoveUnreadAssignments([NotNull] IReadOnlyCollection<VariableName> toRemove, [NotNull] ISingleStaticAssignmentTable ssa)
        {
            _toRemove = toRemove;
            _ssa = ssa;
        }

        protected override BaseStatement Visit(Assignment ass)
        {
            if (_toRemove.Contains(ass.Left))
                return new EmptyStatement();
            else
                return base.Visit(ass);
        }

        protected override BaseStatement Visit(TypedAssignment ass)
        {
            if (_toRemove.Contains(ass.Left))
                return new EmptyStatement();
            else
                return base.Visit(ass);
        }

        protected override BaseStatement Visit(CompoundAssignment compAss)
        {
            if (_toRemove.Contains(compAss.Left))
                return new EmptyStatement();
            else
                return base.Visit(compAss);
        }
    }
}

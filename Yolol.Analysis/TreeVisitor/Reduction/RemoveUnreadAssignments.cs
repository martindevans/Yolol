using System.Collections.Generic;
using System.Linq;
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

        // ReSharper disable once NotAccessedField.Local (this field is included in the constructor as a hint that SSA form is required)
        private readonly ISingleStaticAssignmentTable _ssa;

        public RemoveUnreadAssignments(IReadOnlyCollection<VariableName> toRemove, ISingleStaticAssignmentTable ssa)
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

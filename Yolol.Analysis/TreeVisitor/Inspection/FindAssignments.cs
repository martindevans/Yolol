using System;
using System.Collections.Generic;
using System.Text;
using Yolol.Analysis.ControlFlowGraph.Extensions;
using Yolol.Grammar;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.TreeVisitor.Inspection
{
    class FindAssignments
        : BaseTreeVisitor
    {
        private readonly HashSet<Assignment> _assignments;
        public IReadOnlyCollection<Assignment> Assignments => _assignments;

        // ReSharper disable once NotAccessedField.Local (this field is included in the constructor as a hint that SSA form is required)
        private readonly ISingleStaticAssignmentTable _ssa;

        public FindAssignments(HashSet<Assignment> assignments, ISingleStaticAssignmentTable ssa)
        {
            _assignments = assignments;
            _ssa = ssa;
        }

        protected override BaseStatement Visit(Assignment ass)
        {
            _assignments.Add(ass);

            return base.Visit(ass);
        }
    }
}

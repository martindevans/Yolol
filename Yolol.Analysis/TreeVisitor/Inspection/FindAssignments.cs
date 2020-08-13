using System.Collections.Generic;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.TreeVisitor.Inspection
{
    public class FindAssignments
        : BaseTreeVisitor
    {
        private readonly HashSet<Assignment> _assignments;
        public IReadOnlyCollection<Assignment> Assignments => _assignments;

        public FindAssignments(HashSet<Assignment> assignments)
        {
            _assignments = assignments;
        }

        protected override BaseStatement Visit(Assignment ass)
        {
            _assignments.Add(ass);

            return base.Visit(ass);
        }
    }
}

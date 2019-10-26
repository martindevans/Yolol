using System;
using System.Collections.Generic;
using System.Text;
using Yolol.Analysis.ControlFlowGraph.Extensions;
using Yolol.Grammar;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.TreeVisitor.Inspection
{
    class FindBooleanVariables
        : BaseTreeVisitor
    {
        private readonly HashSet<VariableName> _names;
        public IReadOnlyCollection<VariableName> Names => _names;

        // ReSharper disable once NotAccessedField.Local (this field is included in the constructor as a hint that SSA form is required)
        private readonly ISingleStaticAssignmentTable _ssa;

        public FindBooleanVariables(HashSet<VariableName> names, ISingleStaticAssignmentTable ssa)
        {
            _names = names;
            _ssa = ssa;
        }

        protected override BaseStatement Visit(Assignment ass)
        {
            if (!_names.Contains(ass.Left) && (ass.Right.IsBoolean || (ass.Right is Variable variable && _names.Contains(variable.Name))))
                _names.Add(ass.Left);

            return base.Visit(ass);
        }
    }
}

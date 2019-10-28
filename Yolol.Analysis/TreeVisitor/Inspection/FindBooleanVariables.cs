
using System.Collections.Generic;
using System.Linq;
using Yolol.Analysis.ControlFlowGraph.AST;
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

        private bool IsBoolean(BaseExpression expr)
        {
            if (expr.IsBoolean)
                return true;

            if (expr is Variable variable)
                return _names.Contains(variable.Name);

            if (expr is Phi phi)
                return phi.AssignedNames.All(_names.Contains);

            return false;
        }

        protected override BaseStatement Visit(Assignment ass)
        {
            if (IsBoolean(ass.Right))
                _names.Add(ass.Left);

            return base.Visit(ass);
        }
    }
}

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
        private readonly HashSet<VariableName> _booleanNames;
        public IReadOnlyCollection<VariableName> BooleanNames => _booleanNames;

        private readonly HashSet<VariableName> _nonBooleanNames;
        public IReadOnlyCollection<VariableName> NonBooleanNames => _nonBooleanNames;

        private readonly HashSet<VariableName> _unknownNames;
        public IReadOnlyCollection<VariableName> UnknownNames => _unknownNames;

        // ReSharper disable once NotAccessedField.Local (this field is included in the constructor as a hint that SSA form is required)
        private readonly ISingleStaticAssignmentTable _ssa;

        public FindBooleanVariables(HashSet<VariableName> booleanNames, HashSet<VariableName> nonBooleanNames, HashSet<VariableName> unknownNames, ISingleStaticAssignmentTable ssa)
        {
            _booleanNames = booleanNames;
            _nonBooleanNames = nonBooleanNames;
            _unknownNames = unknownNames;
            _ssa = ssa;
        }

        protected override BaseStatement Visit(Assignment ass)
        {
            if (ass.Right.IsBoolean || ass.Right is Variable booleanVariable && _booleanNames.Contains(booleanVariable.Name))
            {
                _unknownNames.Remove(ass.Left);
                _booleanNames.Add(ass.Left);
            }
            else if (!(ass.Right is Variable) || (ass.Right is Variable nonBooleanVariable && _nonBooleanNames.Contains(nonBooleanVariable.Name)))
            {

                _unknownNames.Remove(ass.Left);
                _nonBooleanNames.Add(ass.Left);
            }
            else
            {
                _unknownNames.Add(ass.Left);
                Console.WriteLine(ass);
            }

            return base.Visit(ass);
        }
    }
}

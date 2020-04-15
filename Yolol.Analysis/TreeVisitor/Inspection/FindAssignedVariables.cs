using System;
using System.Collections.Generic;
using Yolol.Analysis.ControlFlowGraph.AST;
using Yolol.Grammar;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Expressions.Unary;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.TreeVisitor.Inspection
{
    public class FindAssignedVariables
        : BaseTreeVisitor
    {
        private readonly Dictionary<VariableName, uint> _counts = new Dictionary<VariableName, uint>();
        private readonly Dictionary<VariableName, IReadOnlyList<BaseExpression>> _assigned = new Dictionary<VariableName, IReadOnlyList<BaseExpression>>();

        public IEnumerable<VariableName> Names => _counts.Keys;
        public IReadOnlyDictionary<VariableName, uint> Counts => _counts;
        public IReadOnlyDictionary<VariableName, IReadOnlyList<BaseExpression>> Expressions => _assigned;

        private void Add(VariableName name, BaseExpression assignedExpr)
        {
            // Increment count
            _counts.TryGetValue(name, out var value);
            value++;
            _counts[name] = value;

            // Add to assigned set
            if (!_assigned.TryGetValue(name, out var list))
            {
                list = new List<BaseExpression>();
                _assigned.Add(name, list);
            }
            ((List<BaseExpression>)list).Add(assignedExpr);
        }

        protected override BaseStatement Visit(Assignment ass)
        {
            Add(ass.Left, ass.Right);

            return base.Visit(ass);
        }

        protected override BaseStatement Visit(TypedAssignment ass)
        {
            Add(ass.Left, ass.Right);

            return base.Visit(ass);
        }

        protected override BaseStatement Visit(CompoundAssignment compAss)
        {
            Add(compAss.Left, compAss.Right);

            return base.Visit(compAss);
        }

        protected override BaseExpression Visit(PostDecrement dec)
        {
            //Add(dec.Name, dec);
            throw new NotImplementedException();

            return base.Visit(dec);
        }

        protected override BaseExpression Visit(PreDecrement dec)
        {
            //Add(dec.Name);
            throw new NotImplementedException();

            return base.Visit(dec);
        }

        protected override BaseExpression Visit(PostIncrement inc)
        {
            //Add(inc.Name);
            throw new NotImplementedException();

            return base.Visit(inc);
        }

        protected override BaseExpression Visit(PreIncrement inc)
        {
            //Add(inc.Name);
            throw new NotImplementedException();

            return base.Visit(inc);
        }
    }
}

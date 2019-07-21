using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Yolol.Analysis.ControlFlowGraph.AST;
using Yolol.Grammar;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Expressions.Unary;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.TreeVisitor.Inspection
{
    public class FindReadVariables
        : BaseTreeVisitor
    {
        private readonly Dictionary<VariableName, uint> _counts = new Dictionary<VariableName, uint>();

        [NotNull] public IEnumerable<VariableName> Names => _counts.Keys;

        [NotNull] public IEnumerable<(VariableName, uint)> Counts => _counts.Select(a => (a.Key, a.Value));

        private void Add(VariableName name)
        {
            _counts.TryGetValue(name, out var value);
            value++;
            _counts[name] = value;
        }

        protected override BaseExpression Visit(Phi phi)
        {
            foreach (var name in phi.AssignedNames)
                Add(name);

            return phi;
        }

        protected override BaseStatement Visit(CompoundAssignment compAss)
        {
            Add(compAss.Left);

            return base.Visit(compAss);
        }

        protected override BaseExpression Visit(Variable var)
        {
            Add(var.Name);

            return base.Visit(var);
        }

        protected override BaseExpression Visit(PostDecrement dec)
        {
            Add(dec.Name);

            return base.Visit(dec);
        }

        protected override BaseExpression Visit(PreDecrement dec)
        {
            Add(dec.Name);

            return base.Visit(dec);
        }

        protected override BaseExpression Visit(PostIncrement inc)
        {
            Add(inc.Name);

            return base.Visit(inc);
        }

        protected override BaseExpression Visit(PreIncrement inc)
        {
            Add(inc.Name);

            return base.Visit(inc);
        }
    }
}

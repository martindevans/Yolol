using System.Collections.Generic;
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
        private readonly HashSet<VariableName> _names = new HashSet<VariableName>();
        public IEnumerable<VariableName> Names => _names;

        protected override BaseExpression Visit(Phi phi)
        {
            foreach (var name in phi.AssignedNames)
                _names.Add(new VariableName(name));

            return phi;
        }

        protected override BaseStatement Visit(CompoundAssignment compAss)
        {
            _names.Add(compAss.Left);

            return base.Visit(compAss);
        }

        protected override BaseExpression Visit(Variable var)
        {
            _names.Add(var.Name);

            return base.Visit(var);
        }

        protected override BaseExpression Visit(PostDecrement dec)
        {
            _names.Add(dec.Name);

            return base.Visit(dec);
        }

        protected override BaseExpression Visit(PreDecrement dec)
        {
            _names.Add(dec.Name);

            return base.Visit(dec);
        }

        protected override BaseExpression Visit(PostIncrement inc)
        {
            _names.Add(inc.Name);

            return base.Visit(inc);
        }

        protected override BaseExpression Visit(PreIncrement inc)
        {
            _names.Add(inc.Name);

            return base.Visit(inc);
        }
    }
}

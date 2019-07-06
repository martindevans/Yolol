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
        private readonly HashSet<VariableName> _names = new HashSet<VariableName>();
        public IReadOnlyCollection<VariableName> Names => _names;

        protected override BaseExpression VisitUnknown(BaseExpression expression)
        {
            if (expression is Phi phi)
                return phi;

            return base.VisitUnknown(expression);
        }

        protected override BaseStatement VisitUnknown(BaseStatement statement)
        {
            if (statement is Conditional con)
            {
                Visit(con.Condition);
                return con;
            }

            return base.VisitUnknown(statement);
        }

        protected override BaseStatement Visit(Assignment ass)
        {
            _names.Add(ass.Left);

            return base.Visit(ass);
        }

        protected override BaseStatement Visit(CompoundAssignment compAss)
        {
            _names.Add(compAss.Left);

            return base.Visit(compAss);
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

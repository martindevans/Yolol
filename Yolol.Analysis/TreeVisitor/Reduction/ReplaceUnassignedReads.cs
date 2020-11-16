using System.Collections.Generic;
using Yolol.Execution;
using Yolol.Grammar;
using Yolol.Grammar.AST.Expressions;
using Variable = Yolol.Grammar.AST.Expressions.Variable;

namespace Yolol.Analysis.TreeVisitor.Reduction
{
    /// <summary>
    /// Replace reads of unassigned variables with zero
    /// </summary>
    public class ReplaceUnassignedReads
        : BaseTreeVisitor
    {
        private readonly ISet<VariableName> _assigned;

        public ReplaceUnassignedReads(ISet<VariableName> assigned)
        {
            _assigned = assigned;
        }

        protected override BaseExpression Visit(Variable var)
        {
            if (var.Name.IsExternal)
                return var;

            if (_assigned.Contains(var.Name))
                return var;

            return new ConstantNumber((Number)0);
        }
    }
}

using System.Collections.Generic;
using Yolol.Grammar;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Expressions.Unary;

namespace Yolol.Analysis.TreeVisitor.Reduction
{
    public class ReplaceConstantSubexpressions
        : BaseTreeVisitor
    {
        private readonly IReadOnlyDictionary<VariableName, BaseExpression> _constants;

        public ReplaceConstantSubexpressions(IReadOnlyDictionary<VariableName, BaseExpression> constants)
        {
            _constants = constants;
        }

        protected override BaseExpression Visit(Variable var)
        {
            if (_constants.TryGetValue(var.Name, out var subexpression))
                return base.Visit(subexpression);
            else
                return base.Visit(var);
        }
    }
}

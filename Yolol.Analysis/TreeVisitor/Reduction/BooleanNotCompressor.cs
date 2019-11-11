using JetBrains.Annotations;
using System.Collections.Generic;
using Yolol.Grammar;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Expressions.Binary;
using Yolol.Grammar.AST.Expressions.Unary;

namespace Yolol.Analysis.TreeVisitor.Reduction
{
    public class BooleanNotCompressor
        : BaseTreeVisitor
    {
        private readonly ISet<VariableName> _booleans;

        public BooleanNotCompressor([NotNull] ISet<VariableName> booleans)
        {
            _booleans = booleans;
        }

        protected override BaseExpression Visit([NotNull] Not not)
        {
            var p = not.Parameter;

            if (!(p is Variable v))
                return base.Visit(not);

            if (!_booleans.Contains(v.Name))
                return base.Visit(not);

            // Replace `not a` with `(1 - a)`
            return new Bracketed(new Subtract(new ConstantNumber(1), not.Parameter));
        }
    }
}

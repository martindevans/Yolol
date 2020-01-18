using JetBrains.Annotations;
using System.Collections.Generic;
using Yolol.Grammar;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Expressions.Binary;
using Yolol.Grammar.AST.Expressions.Unary;

namespace Yolol.Analysis.TreeVisitor.Reduction
{
    public class BooleanLogicCompressor
        : BaseTreeVisitor
    {
        private readonly ISet<VariableName> _booleans;

        public BooleanLogicCompressor([NotNull] ISet<VariableName> booleans)
        {
            _booleans = booleans;
        }

        protected override BaseExpression Visit(And and)
        {
            var l = base.Visit(and.Left);
            var r = base.Visit(and.Right);

            if (!(l is Variable lv) || !(r is Variable rv))
                return base.Visit(and);

            if (!_booleans.Contains(lv.Name) || !_booleans.Contains(rv.Name))
                return base.Visit(and);

            // Replace `and a b` with `(a * b)`
            return new Bracketed(new Multiply(lv, rv));
        }

        protected override BaseExpression Visit(Or or)
        {
            var l = base.Visit(or.Left);
            var r = base.Visit(or.Right);

            if (!(l is Variable lv) || !(r is Variable rv))
                return base.Visit(or);

            if (!_booleans.Contains(lv.Name) || !_booleans.Contains(rv.Name))
                return base.Visit(or);

            // Replace `or a b` with `(a + b)`
            return new Bracketed(new Add(lv, rv));
        }

        protected override BaseExpression Visit(Not not)
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

using JetBrains.Annotations;
using Yolol.Analysis.ControlFlowGraph.AST;
using Yolol.Analysis.Types;
using Yolol.Execution;
using Yolol.Execution.Extensions;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Expressions.Binary;
using Yolol.Grammar.AST.Expressions.Unary;
using Type = Yolol.Execution.Type;
using Variable = Yolol.Grammar.AST.Expressions.Variable;

namespace Yolol.Analysis.TreeVisitor.Reduction
{
    /// <summary>
    /// Replace trivial mathematical operations including constants with simplified versions (e.g. `x*1 => x`)
    /// </summary>
    public class OpNumByConstNumCompressor
        : BaseTreeVisitor
    {
        private readonly ITypeAssignments _types;

        public OpNumByConstNumCompressor(ITypeAssignments types)
        {
            _types = types;
        }

        protected override BaseExpression Visit(Multiply mul)
        {
            BaseExpression HandleSingleSideNumber(Number number, BaseExpression other)
            {
                switch (number.Value)
                {
                    case -1: return base.Visit(new Negate(new Bracketed(other)));
                    case 0:  return new ConstantNumber(0);
                    case 1:  return base.Visit(other);
                    default: return null;
                }
            }

            // Try to discover value/type for sides
            var lv = DiscoverNumberValue(mul.Left);
            var rv = DiscoverNumberValue(mul.Right);
            var lt = DiscoverType(mul.Left);
            var rt = DiscoverType(mul.Right);

            // multiplications involving strings are errors
            if (lt == Type.String || rt == Type.String)
                return new ErrorExpression();

            // If both sides have a numeric value just statically evaluate
            if (lv.HasValue && rv.HasValue)
                return new ConstantNumber(mul.StaticEvaluate().Number);

            // iAt least one side needs a known value
            if (!lv.HasValue && !rv.HasValue)
                return base.Visit(mul);

            // One side has a known value
            return HandleSingleSideNumber((lv ?? rv).Value, lv.HasValue ? mul.Right : mul.Left) ?? base.Visit(mul);
        }

        protected override BaseExpression Visit(Divide div)
        {
            // Try to discover value/type for sides
            var lv = DiscoverNumberValue(div.Left);
            var rv = DiscoverNumberValue(div.Right);
            var lt = DiscoverType(div.Left);
            var rt = DiscoverType(div.Right);

            // Divisions involving strings are errors
            if (lt == Type.String || rt == Type.String)
                return new ErrorExpression();

            // If both sides have a numeric value just statically evaluate
            if (lv.HasValue && rv.HasValue)
            {
                if (rv.Value == 0)
                    return new ErrorExpression();
                return new ConstantNumber(div.StaticEvaluate().Number);
            }

            // `0/anything` is zero
            if (lv.HasValue && lv.Value == 0)
                return new ConstantNumber(0);

            // if bottom side doesn't have a value we can't optimise
            if (!rv.HasValue)
                return base.Visit(div);

            switch (rv.Value.Value)
            {
                case -1: return base.Visit(new Negate(new Bracketed(base.Visit(div.Left))));
                case 0:  return new ErrorExpression();
                case 1:  return base.Visit(new Bracketed(base.Visit(div.Left)));
                default: return base.Visit(div);
            }
        }

        protected override BaseExpression Visit(Add add)
        {
            BaseExpression HandleSingleSideNumber(Number number, BaseExpression other)
            {
                switch (number.Value)
                {
                    case 0: return base.Visit(new Bracketed(other));
                    default: return null;
                }
            }

            // Try to discover type for sides
            var lt = DiscoverType(add.Left);
            var rt = DiscoverType(add.Right);

            // We aren't optimising string based additions here
            if (lt != Type.Number || rt != Type.Number)
                return base.Visit(add);

            // Get values for the two sides
            var lv = DiscoverNumberValue(add.Left);
            var rv = DiscoverNumberValue(add.Right);

            // Can't do anything without at least one value
            if (!lv.HasValue && !rv.HasValue)
                return base.Visit(add);

            // Replace with constant if both sides are known
            if (lv.HasValue && rv.HasValue)
                return new ConstantNumber(add.StaticEvaluate().Number);

            // One side has a known value
            return HandleSingleSideNumber((lv ?? rv).Value, lv.HasValue ? add.Right : add.Left) ?? add;
        }

        protected override BaseExpression Visit(Exponent exp)
        {
            // Try to discover value/type for sides
            var lv = DiscoverNumberValue(exp.Left);
            var rv = DiscoverNumberValue(exp.Right);
            var lt = DiscoverType(exp.Left);
            var rt = DiscoverType(exp.Right);

            // Exponents involving strings are errors
            if (lt == Type.String || rt == Type.String)
                return new ErrorExpression();

            // If both sides have a numeric value just statically evaluate
            if (lv.HasValue && rv.HasValue)
                return new ConstantNumber(exp.StaticEvaluate().Number);

            // if right side doesn't have a value we can't optimise
            if (!rv.HasValue)
                return base.Visit(exp);

            switch (rv.Value.Value)
            {
                case 0:  return base.Visit(new ConstantNumber(1));
                case 1:  return base.Visit(new Bracketed(base.Visit(exp.Left)));
                case -1: return base.Visit(new Divide(new ConstantNumber(1), new Bracketed(exp.Left)));
                default: return base.Visit(exp);
            }
        }

        protected override BaseExpression Visit(Subtract sub)
        {
            // Try to discover type for sides
            var lt = DiscoverType(sub.Left);
            var rt = DiscoverType(sub.Right);

            // We aren't optimising string based subtraction here
            if (lt != Type.Number || rt != Type.Number)
                return base.Visit(sub);

            // Get values for the two sides
            var lv = DiscoverNumberValue(sub.Left);
            var rv = DiscoverNumberValue(sub.Right);

            // Can't do anything without at least one value
            if (!lv.HasValue && !rv.HasValue)
                return base.Visit(sub);

            // Replace with constant if both sides are known
            if (lv.HasValue && rv.HasValue)
                return new ConstantNumber(sub.StaticEvaluate().Number);

            // If this is `0 - exp` then return `-exp`
            if (lv.HasValue && lv == 0)
                return base.Visit(new Negate(sub.Right));

            // if this is `exp - 0` then return `exp`
            if (rv.HasValue && rv == 0)
                return base.Visit(sub.Left);

            return base.Visit(sub);
        }

        protected override BaseExpression Visit(And and)
        {
            // Try to discover type for sides
            var lt = DiscoverType(and.Left);
            var rt = DiscoverType(and.Right);

            // strings are all `true`, so `str and string` is always true
            if (lt == Type.String && rt == Type.String)
                return new ConstantNumber(1);

            // Get values for the two sides
            var lv = DiscoverNumberValue(and.Left);
            var rv = DiscoverNumberValue(and.Right);

            // Can't do anything without at least one value
            if (!lv.HasValue && !rv.HasValue)
                return base.Visit(and);

            // Replace with constant if both sides are known
            if (lv.HasValue && rv.HasValue)
                return new ConstantNumber(and.StaticEvaluate().Number);

            // If the single known value is zero then this must always evaluate to false
            var v = (lv ?? rv).Value;
            if (v == 0)
                return new ConstantNumber(0);

            // The single known value _is_ not zero, so just return the other side
            return lv.HasValue ? base.Visit(and.Right) : base.Visit(and.Left);
        }

        protected override BaseExpression Visit(Or or)
        {
            // Try to discover type for sides
            var lt = DiscoverType(or.Left);
            var rt = DiscoverType(or.Right);

            // strings are all `true`, so `str or anything` is always true
            if (lt == Type.String || rt == Type.String)
                return new ConstantNumber(1);

            // Get values for the two sides
            var lv = DiscoverNumberValue(or.Left);
            var rv = DiscoverNumberValue(or.Right);

            // Can't do anything without at least one value
            if (!lv.HasValue && !rv.HasValue)
                return base.Visit(or);

            // Replace with constant if both sides are known
            if (lv.HasValue && rv.HasValue)
                return new ConstantNumber(or.StaticEvaluate().Number);

            // If the single known value is _not_ zero then this must always evaluate to true
            var v = (lv ?? rv).Value;
            if (v != 0)
                return new ConstantNumber(1);

            // The single known value _is_ zero, so just return the other side
            return lv.HasValue ? base.Visit(or.Right) : base.Visit(or.Left);
        }

        private Type DiscoverType(BaseExpression expr)
        {
            if (expr is ConstantNumber)
                return Type.Number;

            if (expr is ConstantString)
                return Type.String;

            if (expr is Variable var)
                return _types.TypeOf(var.Name) ?? Type.Unassigned;

            return Type.Unassigned;
        }

        private static Number? DiscoverNumberValue([NotNull] BaseExpression expr)
        {
            if (expr is ConstantNumber con)
                return con.Value;

            if (expr.IsConstant)
            {
                var s = expr.StaticEvaluate();
                if (s.Type == Type.Number)
                    return s.Number;
            }

            return null;
        }
    }
}

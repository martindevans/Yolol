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
            BaseExpression? HandleSingleSideNumber(Number number, BaseExpression other)
            {
                return (long)number switch {
                    -1 => base.Visit(new Negate(new Bracketed(other))),
                    0 => new ConstantNumber((Number)0),
                    1 => base.Visit(other),
                    _ => null
                };
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

            // At least one side needs a known value
            var num = lv ?? rv;
            if (!num.HasValue)
                return base.Visit(mul);

            // One side has a known value
            return HandleSingleSideNumber(num.Value, lv.HasValue ? mul.Right : mul.Left) ?? base.Visit(mul);
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
                if (rv.Value == Number.Zero)
                    return new ErrorExpression();
                return new ConstantNumber(div.StaticEvaluate().Number);
            }

            // `0/anything` is zero
            if (lv.HasValue && lv.Value == Number.Zero)
                return new ConstantNumber((Number)0);

            // if bottom side doesn't have a value we can't optimise
            if (!rv.HasValue)
                return base.Visit(div);

            return (long)rv.Value switch {
                -1 => base.Visit(new Negate(new Bracketed(base.Visit(div.Left)))),
                0 => new ErrorExpression(),
                1 => base.Visit(new Bracketed(base.Visit(div.Left))),
                _ => base.Visit(div)
            };
        }

        protected override BaseExpression Visit(Add add)
        {
            BaseExpression? HandleSingleSideNumber(Number number, BaseExpression other)
            {
                return (long)number switch
                {
                    0 => base.Visit(new Bracketed(other)),
                    _ => null,
                };
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
            return HandleSingleSideNumber((lv ?? rv)!.Value, lv.HasValue ? add.Right : add.Left) ?? add;
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

            return (long)rv.Value switch {
                0 => base.Visit(new ConstantNumber((Number)1)),
                1 => base.Visit(new Bracketed(base.Visit(exp.Left))),
                -1 => base.Visit(new Divide(new ConstantNumber((Number)1), new Bracketed(exp.Left))),
                _ => base.Visit(exp)
            };
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
            if (lv.HasValue && lv == (Number)0)
                return base.Visit(new Negate(sub.Right));

            // if this is `exp - 0` then return `exp`
            if (rv.HasValue && rv == (Number)0)
                return base.Visit(sub.Left);

            return base.Visit(sub);
        }

        protected override BaseExpression Visit(And and)
        {
            var l = DiscoverBoolValue(and.Left, DiscoverType(base.Visit(and.Left)));
            var r = DiscoverBoolValue(and.Right, DiscoverType(base.Visit(and.Right)));

            // Check if either value is true
            if ((l.HasValue && l.Value) && (r.HasValue && r.Value))
                return new ConstantNumber((Number)1);

            // Check if all values are false
            if (l.HasValue && r.HasValue)
                return new ConstantNumber((Number)0);

            return base.Visit(and);
        }

        protected override BaseExpression Visit(Or or)
        {
            var l = DiscoverBoolValue(or.Left, DiscoverType(base.Visit(or.Left)));
            var r = DiscoverBoolValue(or.Right, DiscoverType(base.Visit(or.Right)));

            // Check if all values are false
            if (l.HasValue && !l.Value && r.HasValue && !r.Value)
                return new ConstantNumber((Number)0);

            // Check if either value is true
            if ((l.HasValue && l.Value) || (r.HasValue && r.Value))
                return new ConstantNumber((Number)1);

            return base.Visit(or);
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

        private static bool? DiscoverBoolValue(BaseExpression expr, Type? type)
        {
            if (type.HasValue && type.Value == Type.String)
                return true;

            var n = DiscoverNumberValue(expr);
            if (n.HasValue)
                return n != (Number)0;

            var s = DiscoverStringValue(expr);
            if (s != null)
                return true;

            return null;
        }

        private static Number? DiscoverNumberValue(BaseExpression expr)
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

        private static string? DiscoverStringValue(BaseExpression expr)
        {
            if (expr is ConstantString con)
                return con.Value.ToString();

            if (expr.IsConstant)
            {
                var s = expr.StaticEvaluate();
                if (s.Type == Type.String)
                    return s.String.ToString();
            }

            return null;
        }
    }
}

using System;
using JetBrains.Annotations;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Expressions.Binary;

namespace Yolol.Grammar
{
    public enum YololBinaryOp
    {
        Add,
        Subtract,
        Multiply,
        Divide,
        Modulo,
        Exponent,

        LessThan,
        GreaterThan,
        LessThanEqualTo,
        GreaterThanEqualTo,
        NotEqualTo,
        EqualTo,

        And,
        Or
    }

    public static class YololBinaryOpExtensions
    {
        [NotNull] public static BaseBinaryExpression ToExpression(this YololBinaryOp op, [NotNull] BaseExpression left, [NotNull] BaseExpression right)
        {
            switch (op)
            {
                case YololBinaryOp.Add: return new Add(left, right);
                case YololBinaryOp.Subtract: return new Subtract(left, right);
                case YololBinaryOp.Multiply: return new Multiply(left, right);
                case YololBinaryOp.Divide: return new Divide(left, right);
                case YololBinaryOp.Modulo: return new Modulo(left, right);
                case YololBinaryOp.Exponent: return new Exponent(left, right);
                case YololBinaryOp.LessThan: return new LessThan(left, right);
                case YololBinaryOp.GreaterThan: return new GreaterThan(left, right);
                case YololBinaryOp.LessThanEqualTo: return new LessThanEqualTo(left, right);
                case YololBinaryOp.GreaterThanEqualTo: return new GreaterThanEqualTo(left, right);
                case YololBinaryOp.NotEqualTo: return new NotEqualTo(left, right);
                case YololBinaryOp.EqualTo: return new EqualTo(left, right);
                case YololBinaryOp.And: return new And(left, right);
                case YololBinaryOp.Or: return new Or(left, right);

                //ncrunch: no coverage start
                default:
                    throw new ArgumentOutOfRangeException(nameof(op), op, null);
                //ncrunch: no coverage end
            }
        }

        [NotNull] public static string String(this YololBinaryOp op)
        {
            switch (op)
            {
                case YololBinaryOp.Add: return "+";
                case YololBinaryOp.Subtract: return "-";
                case YololBinaryOp.Multiply: return "*";
                case YololBinaryOp.Divide: return "/";
                case YololBinaryOp.Modulo: return "%";
                case YololBinaryOp.Exponent: return "^";
                case YololBinaryOp.LessThan: return "<";
                case YololBinaryOp.GreaterThan: return ">";
                case YololBinaryOp.LessThanEqualTo: return "<=";
                case YololBinaryOp.GreaterThanEqualTo: return ">=";
                case YololBinaryOp.NotEqualTo: return "!=";
                case YololBinaryOp.EqualTo: return "==";
                case YololBinaryOp.And: return "and";
                case YololBinaryOp.Or: return "or";

                //ncrunch: no coverage start
                default:
                    throw new ArgumentOutOfRangeException(nameof(op), op, null);
                //ncrunch: no coverage end
            }
        }
    }
}

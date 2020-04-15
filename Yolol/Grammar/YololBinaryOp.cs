using System;
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
        public static BaseBinaryExpression ToExpression(this YololBinaryOp op, BaseExpression left, BaseExpression right)
        {
            return op switch {
                YololBinaryOp.Add => new Add(left, right),
                YololBinaryOp.Subtract => new Subtract(left, right),
                YololBinaryOp.Multiply => new Multiply(left, right),
                YololBinaryOp.Divide => new Divide(left, right),
                YololBinaryOp.Modulo => new Modulo(left, right),
                YololBinaryOp.Exponent => new Exponent(left, right),
                YololBinaryOp.LessThan => new LessThan(left, right),
                YololBinaryOp.GreaterThan => new GreaterThan(left, right),
                YololBinaryOp.LessThanEqualTo => new LessThanEqualTo(left, right),
                YololBinaryOp.GreaterThanEqualTo => new GreaterThanEqualTo(left, right),
                YololBinaryOp.NotEqualTo => new NotEqualTo(left, right),
                YololBinaryOp.EqualTo => new EqualTo(left, right),
                YololBinaryOp.And => new And(left, right),
                YololBinaryOp.Or => new Or(left, right),
                _ => throw new ArgumentOutOfRangeException(nameof(op), op, null)
            };
        }

        public static string String(this YololBinaryOp op)
        {
            return op switch {
                YololBinaryOp.Add => "+",
                YololBinaryOp.Subtract => "-",
                YololBinaryOp.Multiply => "*",
                YololBinaryOp.Divide => "/",
                YololBinaryOp.Modulo => "%",
                YololBinaryOp.Exponent => "^",
                YololBinaryOp.LessThan => "<",
                YololBinaryOp.GreaterThan => ">",
                YololBinaryOp.LessThanEqualTo => "<=",
                YololBinaryOp.GreaterThanEqualTo => ">=",
                YololBinaryOp.NotEqualTo => "!=",
                YololBinaryOp.EqualTo => "==",
                YololBinaryOp.And => "and",
                YololBinaryOp.Or => "or",
                _ => throw new ArgumentOutOfRangeException(nameof(op), op, null)
            };
        }
    }
}

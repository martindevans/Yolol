using System;

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
    }

    public static class YololBinaryOpExtensions
    {
        public static string String(this YololBinaryOp op)
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
                case YololBinaryOp.NotEqualTo: return "~=";
                case YololBinaryOp.EqualTo: return "==";
                default:
                    throw new ArgumentOutOfRangeException(nameof(op), op, null);
            }
        }
    }
}

using System;
using YololEmulator.Execution;
using YololEmulator.Grammar.AST.Expressions.Binary;

namespace YololEmulator.Grammar.AST.Expressions
{
    public abstract class BaseExpression
    {
        public abstract Value Evaluate(MachineState state);

        public static BaseExpression MakeBinary(YololBinaryOp operand, BaseExpression lhs, BaseExpression rhs)
        {
            switch (operand)
            {
                case YololBinaryOp.Add:
                    return new AddExpression(lhs, rhs);
                case YololBinaryOp.Subtract:
                    return new SubtractExpression(lhs, rhs);
                case YololBinaryOp.Multiply:
                    return new MultiplyExpression(lhs, rhs);
                case YololBinaryOp.Divide:
                    return new DivideExpression(lhs, rhs);
                default:
                    throw new ArgumentOutOfRangeException(nameof(operand), operand, null);
            }
        }
    }
}

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
                case YololBinaryOp.LessThan:
                    return new LessThanExpression(lhs, rhs);
                case YololBinaryOp.GreaterThan:
                    return new GreaterThanExpression(lhs, rhs);
                case YololBinaryOp.LessThanEqualTo:
                    return new LessThanEqualToExpression(lhs, rhs);
                case YololBinaryOp.GreaterThanEqualTo:
                    return new GreaterThanEqualToExpression(lhs, rhs);
                case YololBinaryOp.NotEqualTo:
                    return new NotEqualToExpression(lhs, rhs);
                case YololBinaryOp.EqualTo:
                    return new EqualToExpression(lhs, rhs);

                //ncrunch: no coverage start
                default:
                    throw new ArgumentOutOfRangeException(nameof(operand), operand, null);
                //ncrunch: no coverage end
            }
        }
    }
}

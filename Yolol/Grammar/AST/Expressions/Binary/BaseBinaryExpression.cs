using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Binary
{
    public abstract class BaseBinaryExpression
        : BaseExpression
    {
        public BaseExpression Left { get; }
        public BaseExpression Right { get; }

        public override bool IsConstant => Left.IsConstant && Right.IsConstant;

        public override bool IsBoolean => false;

        protected BaseBinaryExpression(BaseExpression left, BaseExpression right)
        {
            Left = left;
            Right = right;
        }

        protected abstract Value Evaluate(Value left, Value right);

        public override Value Evaluate(MachineState state)
        {
            var l = Left.Evaluate(state);
            var r = Right.Evaluate(state);

            return Evaluate(l, r);
        }

        public static BaseExpression Create(YololBinaryOp op, BaseExpression lhs, BaseExpression rhs)
        {
            return op.ToExpression(lhs, rhs);
        }

        public override int GetHashCode()
        {
            return unchecked(Left.GetHashCode() * Right.GetHashCode());
        }
    }
}

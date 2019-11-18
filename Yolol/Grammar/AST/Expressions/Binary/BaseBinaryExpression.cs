using JetBrains.Annotations;
using Yolol.Execution;
using Type = Yolol.Execution.Type;

namespace Yolol.Grammar.AST.Expressions.Binary
{
    public abstract class BaseBinaryExpression
        : BaseExpression
    {
        [NotNull] public BaseExpression Left { get; }
        [NotNull] public BaseExpression Right { get; }

        public override bool IsConstant => Left.IsConstant && Right.IsConstant;

        public override bool IsBoolean => false;

        protected BaseBinaryExpression([NotNull] BaseExpression left, [NotNull] BaseExpression right)
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

        [NotNull] public static BaseExpression Create(YololBinaryOp op, [NotNull] BaseExpression lhs, [NotNull] BaseExpression rhs)
        {
            return op.ToExpression(lhs, rhs);
        }
    }
}

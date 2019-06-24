using JetBrains.Annotations;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Binary
{
    public abstract class BaseBinaryExpression
        : BaseExpression
    {
        [NotNull] public BaseExpression Left { get; }
        [NotNull] public BaseExpression Right { get; }

        public override bool IsConstant => Left.IsConstant && Right.IsConstant;

        protected BaseBinaryExpression([NotNull] BaseExpression left, [NotNull] BaseExpression right)
        {
            Left = left;
            Right = right;
        }

        protected abstract Value Evaluate([NotNull] string l, [NotNull] string r);

        protected abstract Value Evaluate(Number l, Number r);

        protected abstract Value Evaluate([NotNull] string l, Number r);

        protected abstract Value Evaluate(Number l, [NotNull] string r);

        public override Value Evaluate(MachineState state)
        {
            var l = Left.Evaluate(state);
            var r = Right.Evaluate(state);

            if (l.Type == Type.Number && r.Type == Type.Number)
                return Evaluate(l.Number, r.Number);

            if (l.Type == Type.String && r.Type == Type.String)
                return Evaluate(l.String, r.String);

            if (l.Type == Type.Number)
                return Evaluate(l.Number, r.String);

            return Evaluate(l.String, r.Number);
        }

        [NotNull] public static BaseExpression Create(YololBinaryOp op, [NotNull] BaseExpression lhs, [NotNull] BaseExpression rhs)
        {
            return op.ToExpression(lhs, rhs);
        }
    }
}

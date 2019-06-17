using YololEmulator.Execution;

namespace YololEmulator.Grammar.AST.Expressions.Binary
{
    public abstract class BaseBinaryExpression
        : BaseExpression
    {
        public BaseExpression Left { get; }
        public BaseExpression Right { get; }

        protected BaseBinaryExpression(BaseExpression left, BaseExpression right)
        {
            Left = left;
            Right = right;
        }

        protected abstract Value Evaluate(string l, string r);

        protected abstract Value Evaluate(Number l, Number r);

        protected abstract Value Evaluate(string l, Number r);

        protected abstract Value Evaluate(Number l, string r);

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
    }
}

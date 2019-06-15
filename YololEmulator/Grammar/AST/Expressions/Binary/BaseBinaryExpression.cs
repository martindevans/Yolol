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

        protected abstract Value Evaluate(decimal l, decimal r);

        protected abstract Value Evaluate(string l, decimal r);

        protected abstract Value Evaluate(decimal l, string r);

        public override Value Evaluate(MachineState state)
        {
            var l = Left.Evaluate(state);
            var r = Right.Evaluate(state);

            if (l.Type == Type.None || r.Type == Type.None)
                throw new ExecutionError("Attempted to operate on an unassigned value");

            if (l.Type == Type.Number && r.Type == Type.Number)
                return Evaluate(l.ValueNumber, r.ValueNumber);

            if (l.Type == Type.String && r.Type == Type.String)
                return Evaluate(l.ValueString, r.ValueString);

            if (l.Type == Type.Number)
                return Evaluate(l.ValueNumber, r.ValueString);

            return Evaluate(l.ValueString, r.ValueNumber);
        }
    }
}

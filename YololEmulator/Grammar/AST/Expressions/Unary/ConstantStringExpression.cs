using YololEmulator.Execution;

namespace YololEmulator.Grammar.AST.Expressions.Unary
{
    public class ConstantString
        : BaseExpression
    {
        public string Value { get; }

        public ConstantString(string value)
        {
            Value = value;
        }

        public override Value Evaluate(MachineState _)
        {
            return new Value(Value);
        }
    }
}

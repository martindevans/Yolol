using YololEmulator.Execution;

namespace YololEmulator.Grammar.AST.Expressions.Unary
{
    public class ConstantNumber
        : BaseExpression
    {
        public decimal Value { get; }

        public ConstantNumber(decimal value)
        {
            Value = value;
        }

        public override Value Evaluate(MachineState _)
        {
            return new Value(Value);
        }
    }
}

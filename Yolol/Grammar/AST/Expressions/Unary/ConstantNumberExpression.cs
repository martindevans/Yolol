using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class ConstantNumber
        : BaseExpression
    {
        public Number Value { get; }

        public ConstantNumber(Number value)
        {
            Value = value;
        }

        public override Value Evaluate(MachineState _)
        {
            return new Value(Value);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}

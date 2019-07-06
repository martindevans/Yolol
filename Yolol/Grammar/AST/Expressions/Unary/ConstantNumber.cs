using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class ConstantNumber
        : BaseExpression
    {
        public Number Value { get; }

        public override bool CanRuntimeError => false;

        public override bool IsBoolean => Value == 0 || Value == 1;

        public override bool IsConstant => true;

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

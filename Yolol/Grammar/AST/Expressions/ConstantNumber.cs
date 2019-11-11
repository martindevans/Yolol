using System;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions
{
    public class ConstantNumber
        : BaseExpression, IEquatable<ConstantNumber>
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

        public bool Equals(ConstantNumber other)
        {
            return other != null
                && other.Value.Equals(Value);
        }

        public override bool Equals(BaseExpression other)
        {
            return other is ConstantNumber num
                && num.Equals(this);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}

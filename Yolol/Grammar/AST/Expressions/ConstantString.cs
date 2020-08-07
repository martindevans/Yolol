using System;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions
{
    public class ConstantString
        : BaseExpression, IEquatable<ConstantString>
    {
        public YString Value { get; }

        public override bool CanRuntimeError => false;

        public override bool IsBoolean => false;

        public override bool IsConstant => true;

        public ConstantString(string value)
        {
            Value = new YString(value);
        }

        public ConstantString(YString value)
        {
            Value = value;
        }

        public override Value Evaluate(MachineState _)
        {
            return new Value(Value);
        }

        public bool Equals(ConstantString? other)
        {
            return other != null
                && other.Value.Equals(Value);
        }

        public override bool Equals(BaseExpression? other)
        {
            return other is ConstantString str
                && str.Equals(this);
        }

        public override string ToString()
        {
            return $"\"{Value}\"";
        }
    }
}

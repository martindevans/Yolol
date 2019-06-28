using JetBrains.Annotations;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class ConstantString
        : BaseExpression
    {
        [NotNull] public string Value { get; }

        public override bool IsBoolean => false;

        public override bool IsConstant => true;

        public ConstantString([NotNull] string value)
        {
            Value = value;
        }

        public override Value Evaluate(MachineState _)
        {
            return new Value(Value);
        }

        public override string ToString()
        {
            return $"\"{Value}\"";
        }
    }
}

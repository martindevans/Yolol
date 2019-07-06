using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class Variable
        : BaseExpression
    {
        public VariableName Name { get; }

        public override bool CanRuntimeError => false;

        public override bool IsBoolean => false;

        public override bool IsConstant => false;

        public Variable(VariableName name)
        {
            Name = name;
        }

        public override Value Evaluate(MachineState state)
        {
            return state.GetVariable(Name.Name).Value;
        }

        public override string ToString()
        {
            return Name.Name;
        }
    }
}

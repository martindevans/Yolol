using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class VariableExpression
        : BaseExpression
    {
        public VariableName Variable { get; }

        public VariableExpression(VariableName variable)
        {
            Variable = variable;
        }

        public override Value Evaluate(MachineState state)
        {
            return state.GetVariable(Variable.Name).Value;
        }

        public override string ToString()
        {
            return Variable.Name;
        }
    }
}

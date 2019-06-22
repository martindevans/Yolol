using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class VariableExpression
        : BaseExpression
    {
        private readonly VariableName _var;

        public VariableExpression(VariableName var)
        {
            _var = var;
        }

        public override Value Evaluate(MachineState state)
        {
            return state.GetVariable(_var.Name).Value;
        }

        public override string ToString()
        {
            return _var.Name;
        }
    }
}

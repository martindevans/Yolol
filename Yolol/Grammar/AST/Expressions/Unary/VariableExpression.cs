using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class VariableExpression
        : BaseExpression
    {
        private readonly string _name;

        public VariableExpression(string name)
        {
            _name = name;
        }

        public override Value Evaluate(MachineState state)
        {
            return state.GetVariable(_name).Value;
        }

        public override string ToString()
        {
            return _name;
        }
    }
}

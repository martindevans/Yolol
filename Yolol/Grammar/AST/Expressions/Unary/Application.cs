using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class Application
        : BaseExpression
    {
        private readonly string _name;
        private readonly BaseExpression _rhs;

        public Application(string name, BaseExpression rhs)
        {
            _name = name;
            _rhs = rhs;
        }

        public override Value Evaluate(MachineState state)
        {
            var intrinsic = state.GetIntrinsic(_name);
            if (intrinsic == null)
                throw new ExecutionException("Attempted to call unknown function `{_name}`");

            return intrinsic(_rhs.Evaluate(state));
        }
    }
}

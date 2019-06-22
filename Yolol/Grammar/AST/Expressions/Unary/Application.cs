using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class Application
        : BaseExpression
    {
        public FunctionName Name { get; }
        public BaseExpression Parameter { get; }

        public Application(FunctionName name, BaseExpression parameter)
        {
            Name = name;
            Parameter = parameter;
        }

        public override Value Evaluate(MachineState state)
        {
            var intrinsic = state.GetIntrinsic(Name.Name);
            if (intrinsic == null)
                throw new ExecutionException("Attempted to call unknown function `{_name}`");

            return intrinsic(Parameter.Evaluate(state));
        }

        public override string ToString()
        {
            return $"{Name}({Parameter})";
        }
    }
}

using JetBrains.Annotations;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class Application
        : BaseExpression
    {
        [NotNull] public FunctionName Name { get; }
        [NotNull] public BaseExpression Parameter { get; }

        public override bool IsConstant => Parameter.IsConstant;

        public Application([NotNull] FunctionName name, [NotNull] BaseExpression parameter)
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

using Yolol.Execution;
using Yolol.Grammar.AST.Expressions;

namespace Yolol.Grammar.AST.Statements
{
    public class Assignment
        : BaseStatement
    {
        public override bool CanRuntimeError => Right.CanRuntimeError;

        public VariableName Left { get; }
        public BaseExpression Right { get; }

        public Assignment(VariableName left, BaseExpression right)
        {
            Left = left;
            Right = right;
        }

        public override ExecutionResult Evaluate(MachineState state)
        {
            var var = state.GetVariable(Left.Name);
            var.Value = Right.Evaluate(state);

            return new ExecutionResult();
        }

        public override string ToString()
        {
            return $"{Left}={Right}";
        }
    }
}

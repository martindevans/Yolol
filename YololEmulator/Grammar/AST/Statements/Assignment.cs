using YololEmulator.Execution;
using YololEmulator.Grammar.AST.Expressions;

namespace YololEmulator.Grammar.AST.Statements
{
    public class Assignment
        : BaseStatement
    {
        public VariableName Left { get; }
        public BaseExpression Right { get; }

        public Assignment(VariableName left, BaseExpression right)
        {
            Left = left;
            Right = right;
        }

        public override ExecutionResult Evaluate(MachineState state)
        {
            var var = state.Get(Left.Name);
            var.Value = Right.Evaluate(state);

            return new ExecutionResult();
        }
    }
}

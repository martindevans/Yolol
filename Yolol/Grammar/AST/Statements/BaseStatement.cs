using Yolol.Execution;

namespace Yolol.Grammar.AST.Statements
{
    public abstract class BaseStatement
    {
        public abstract ExecutionResult Evaluate(MachineState state);
    }
}

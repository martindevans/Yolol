using YololEmulator.Execution;

namespace YololEmulator.Grammar.AST.Statements
{
    public abstract class BaseStatement
    {
        public abstract ExecutionResult Evaluate(MachineState state);
    }
}

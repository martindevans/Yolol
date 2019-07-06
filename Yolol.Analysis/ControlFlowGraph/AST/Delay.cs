using Yolol.Execution;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.ControlFlowGraph.AST
{
    /// <summary>
    /// Delay execution by one line
    /// </summary>
    public class Delay
        : BaseStatement
    {
        public override bool CanRuntimeError => false;

        public override ExecutionResult Evaluate(MachineState state)
        {
            return new ExecutionResult();
        }

        public override string ToString()
        {
            return "delay()";
        }
    }
}

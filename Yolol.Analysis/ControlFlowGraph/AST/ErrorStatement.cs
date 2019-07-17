using Yolol.Execution;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.ControlFlowGraph.AST
{
    public class ErrorStatement
        : BaseStatement
    {
        public override bool CanRuntimeError => true;
        public override ExecutionResult Evaluate(MachineState state)
        {
            throw new ExecutionException("Static error");
        }

        public override string ToString()
        {
            return "error()";
        }
    }
}

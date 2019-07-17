using Yolol.Execution;
using Yolol.Grammar.AST.Expressions;

namespace Yolol.Analysis.ControlFlowGraph.AST
{
    public class ErrorExpression
        : BaseExpression
    {
        public override bool IsConstant => true;
        public override bool IsBoolean => false;
        public override bool CanRuntimeError => true;
        public override Value Evaluate(MachineState state)
        {
            throw new ExecutionException("Static error");
        }

        public override string ToString()
        {
            return "error()";
        }
    }
}

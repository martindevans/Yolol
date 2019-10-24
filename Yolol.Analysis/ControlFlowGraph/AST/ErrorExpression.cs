using System;
using Yolol.Execution;
using Yolol.Grammar.AST.Expressions;

namespace Yolol.Analysis.ControlFlowGraph.AST
{
    public class ErrorExpression
        : BaseExpression, IEquatable<ErrorExpression>
    {
        public override bool IsConstant => true;
        public override bool IsBoolean => false;
        public override bool CanRuntimeError => true;
        public override Value Evaluate(MachineState state)
        {
            throw new ExecutionException("Static error");
        }

        public bool Equals(ErrorExpression other)
        {
            return other != null;
        }

        public override bool Equals(BaseExpression other)
        {
            return other is ErrorExpression a
                && a.Equals(this);
        }

        public override string ToString()
        {
            return "error()";
        }
    }
}

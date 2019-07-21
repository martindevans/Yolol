using System;
using JetBrains.Annotations;
using Yolol.Execution;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.ControlFlowGraph.AST
{
    public class ErrorStatement
        : BaseStatement, IEquatable<ErrorStatement>
    {
        public override bool CanRuntimeError => true;
        public override ExecutionResult Evaluate(MachineState state)
        {
            throw new ExecutionException("Static error");
        }

        public bool Equals([CanBeNull] ErrorStatement other)
        {
            return other != null;
        }

        public override bool Equals(BaseStatement other)
        {
            return other is ErrorStatement a
                && a.Equals(this);
        }

        public override string ToString()
        {
            return "error()";
        }
    }
}

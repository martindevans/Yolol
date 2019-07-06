using System;
using JetBrains.Annotations;
using Yolol.Execution;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.ControlFlowGraph.AST
{
    /// <summary>
    /// Evaluate a conditional
    /// </summary>
    public class Conditional
        : BaseStatement
    {
        public override bool CanRuntimeError => Condition.CanRuntimeError;

        [NotNull] public BaseExpression Condition { get; }

        public Conditional([NotNull] BaseExpression condition)
        {
            Condition = condition;
        }

        public override ExecutionResult Evaluate(MachineState state)
        {
            throw new InvalidOperationException("Cannot execute `Conditional` node");
        }

        public override string ToString()
        {
            return $"conditional({Condition})";
        }
    }
}

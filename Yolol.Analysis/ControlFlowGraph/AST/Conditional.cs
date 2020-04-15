using System;
using Yolol.Execution;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.ControlFlowGraph.AST
{
    /// <summary>
    /// Evaluate a conditional
    /// </summary>
    public class Conditional
        : BaseStatement, IEquatable<Conditional>
    {
        public override bool CanRuntimeError => Condition.CanRuntimeError;

        public BaseExpression Condition { get; }

        public Conditional(BaseExpression condition)
        {
            Condition = condition;
        }

        public override ExecutionResult Evaluate(MachineState state)
        {
            throw new InvalidOperationException("Cannot execute `Conditional` node");
        }

        public bool Equals(Conditional other)
        {
            return other != null
                && other.Condition.Equals(Condition);
        }

        public override bool Equals(BaseStatement other)
        {
            return other is Conditional a
                && a.Equals(this);
        }

        public override string ToString()
        {
            return $"conditional({Condition})";
        }
    }
}

using System;
using JetBrains.Annotations;
using Yolol.Execution;
using Yolol.Grammar;
using Yolol.Grammar.AST.Expressions;

namespace Yolol.Analysis.ControlFlowGraph.AST
{
    /// <summary>
    /// Performs the increment operation on a value without modifying the underlying field
    /// </summary>
    public class Increment
        : BaseExpression
    {
        public VariableName Name { get; }

        public override bool IsConstant => false;
        public override bool IsBoolean => false;
        public override bool CanRuntimeError => false;

        public Increment(VariableName name)
        {
            Name = name;
        }

        public override Value Evaluate(MachineState state)
        {
            throw new InvalidOperationException("Cannot execute `Increment` node");
        }

        public bool Equals([CanBeNull] Increment other)
        {
            return other != null
                && other.Name.Equals(Name);
        }

        public override bool Equals(BaseExpression other)
        {
            return other is Increment a
                && a.Equals(this);
        }

        public override string ToString()
        {
            return $"inc({Name.Name})";
        }
    }
}

using System;
using Yolol.Execution;
using Yolol.Grammar;
using Yolol.Grammar.AST.Expressions;

namespace Yolol.Analysis.ControlFlowGraph.AST
{
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

        public override string ToString()
        {
            return $"inc({Name.Name})";
        }
    }
}

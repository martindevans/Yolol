using System;
using Yolol.Execution;
using Yolol.Grammar;
using Yolol.Grammar.AST.Expressions;

namespace Yolol.Analysis.ControlFlowGraph.AST
{
    /// <summary>
    /// Performs the increment operation on a value without modifying the underlying field
    /// </summary>
    public class Increment
        : BaseExpression, IEquatable<Increment>
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

            //var v = state.GetVariable(Name.Name).Value;
            //v++;
            //return v;
        }

        public bool Equals(Increment other)
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

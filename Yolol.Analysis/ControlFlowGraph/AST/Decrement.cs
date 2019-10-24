using System;
using Yolol.Execution;
using Yolol.Grammar;
using Yolol.Grammar.AST.Expressions;

namespace Yolol.Analysis.ControlFlowGraph.AST
{
    /// <summary>
    /// Performs the decrement operation on a value without modifying the underlying field
    /// </summary>
    public class Decrement
        : BaseExpression, IEquatable<Decrement>
    {
        public VariableName Name { get; }

        public override bool IsConstant => false;
        public override bool IsBoolean => false;
        public override bool CanRuntimeError => false;

        public Decrement(VariableName name)
        {
            Name = name;
        }

        public override Value Evaluate(MachineState state)
        {
            throw new InvalidOperationException("Cannot execute `Decrement` node");
        }

        public bool Equals(Decrement other)
        {
            return other != null
                && other.Name.Equals(Name);
        }

        public override bool Equals(BaseExpression other)
        {
            return other is Decrement post
                && post.Equals(this);
        }

        public override string ToString()
        {
            return $"dec({Name.Name})";
        }
    }
}

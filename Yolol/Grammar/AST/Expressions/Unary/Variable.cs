using System;
using JetBrains.Annotations;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class Variable
        : BaseExpression, IEquatable<Variable>
    {
        public VariableName Name { get; }

        public override bool CanRuntimeError => false;

        public override bool IsBoolean => false;

        public override bool IsConstant => false;

        public Variable(VariableName name)
        {
            Name = name;
        }

        public override Value Evaluate(MachineState state)
        {
            return state.GetVariable(Name.Name).Value;
        }

        public bool Equals(Variable other)
        {
            return other != null
                && other.Name.Equals(Name);
        }

        public override bool Equals(BaseExpression other)
        {
            return other is Variable var
                && var.Equals(this);
        }

        public override string ToString()
        {
            return Name.Name;
        }
    }
}

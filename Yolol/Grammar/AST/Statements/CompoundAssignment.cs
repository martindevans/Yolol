using System;
using JetBrains.Annotations;
using Yolol.Execution;
using Yolol.Grammar.AST.Expressions;
using Variable = Yolol.Grammar.AST.Expressions.Unary.Variable;

namespace Yolol.Grammar.AST.Statements
{
    public class CompoundAssignment
        : Assignment, IEquatable<CompoundAssignment>
    {
        public YololBinaryOp Op { get; }
        public BaseExpression Expression { get; }

        public CompoundAssignment(VariableName variable, YololBinaryOp op, BaseExpression expression)
            : base(variable, op.ToExpression(new Variable(variable), expression))
        {
            Op = op;
            Expression = expression;
        }

        //public override ExecutionResult Evaluate(MachineState state)
        //{
        //    var var = state.GetVariable(Left.Name);
        //    var.Value = Right.Evaluate(state);

        //    return new ExecutionResult();
        //}

        public bool Equals([CanBeNull] CompoundAssignment other)
        {
            return Equals((Assignment)other);
        }

        public override string ToString()
        {
            return $"{Left.Name}{Op.String()}={Expression}";
        }
    }
}

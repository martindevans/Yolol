using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Expressions.Unary;

namespace Yolol.Grammar.AST.Statements
{
    public class CompoundAssignment
        : Assignment
    {
        private readonly VariableName _var;
        private readonly YololBinaryOp _op;
        private readonly BaseExpression _rhs;

        public CompoundAssignment(VariableName var, YololBinaryOp op, BaseExpression rhs)
            : base(var, BaseExpression.MakeBinary(op, new VariableExpression(var.Name), rhs))
        {
            _var = var;
            _op = op;
            _rhs = rhs;
        }

        public override string ToString()
        {
            return $"{_var.Name}{_op.String()}={_rhs}";
        }
    }
}

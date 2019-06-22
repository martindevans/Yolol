using Yolol.Execution;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Expressions.Unary;

namespace Yolol.Grammar.AST.Statements
{
    public class CompoundAssignment
        : Assignment
    {
        public YololBinaryOp Op { get; }
        public BaseExpression Expression { get; }

        public CompoundAssignment(VariableName variable, YololBinaryOp op, BaseExpression expression)
            : base(variable, BaseExpression.MakeBinary(op, new VariableExpression(variable), expression))
        {
            Op = op;
            Expression = expression;
        }

        public override string ToString()
        {
            return $"{Left.Name}{Op.String()}={Expression}";
        }
    }
}

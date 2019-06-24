using Yolol.Grammar.AST.Expressions;
using Variable = Yolol.Grammar.AST.Expressions.Unary.Variable;

namespace Yolol.Grammar.AST.Statements
{
    public class CompoundAssignment
        : Assignment
    {
        public YololBinaryOp Op { get; }
        public BaseExpression Expression { get; }

        public CompoundAssignment(VariableName variable, YololBinaryOp op, BaseExpression expression)
            : base(variable, op.ToExpression(new Variable(variable), expression))
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

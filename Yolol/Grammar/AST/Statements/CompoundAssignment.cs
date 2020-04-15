using System;
using Yolol.Grammar.AST.Expressions;
using Variable = Yolol.Grammar.AST.Expressions.Variable;

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

        public bool Equals(CompoundAssignment? other)
        {
            return Equals(other as Assignment);
        }

        public override bool Equals(BaseStatement? other)
        {
            return other is CompoundAssignment ass
                && ass.Equals(this);
        }

        public override string ToString()
        {
            return $"{Left.Name}{Op.String()}={Expression}";
        }
    }
}

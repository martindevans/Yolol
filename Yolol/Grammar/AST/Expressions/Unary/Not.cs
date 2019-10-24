using System;
using JetBrains.Annotations;
using Yolol.Execution;
using Type = Yolol.Execution.Type;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class Not
        : BaseExpression, IEquatable<Not>
    {
        [NotNull] public BaseExpression Expression { get; }

        public override bool CanRuntimeError => Expression.CanRuntimeError;

        public override bool IsBoolean => true;

        public override bool IsConstant => Expression.IsConstant;

        public Not([NotNull] BaseExpression expression)
        {
            Expression = expression;
        }

        public override Value Evaluate(MachineState state)
        {
            var v = Expression.Evaluate(state);

            if (v.Type == Type.String)
                return new Value(false);

            return new Value(v.Number == 0);
        }

        public bool Equals(Not other)
        {
            return other != null
                && other.Expression.Equals(Expression);
        }

        public override bool Equals(BaseExpression other)
        {
            return other is Not not
                && not.Equals(this);
        }

        public override string ToString()
        {
            return $"not {Expression}";
        }
    }
}

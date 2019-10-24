using System;
using JetBrains.Annotations;
using Yolol.Execution;
using Yolol.Grammar.AST.Expressions;

namespace Yolol.Grammar.AST.Statements
{
    public class ExpressionWrapper
        : BaseStatement, IEquatable<ExpressionWrapper>
    {
        [NotNull] public BaseExpression Expression { get; }

        public override bool CanRuntimeError => Expression.CanRuntimeError;

        public ExpressionWrapper([NotNull] BaseExpression expression)
        {
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
        }

        public override ExecutionResult Evaluate(MachineState state)
        {
            Expression.Evaluate(state);

            return new ExecutionResult();
        }

        public bool Equals(ExpressionWrapper other)
        {
            return other != null
                && other.Expression.Equals(Expression);
        }

        public override bool Equals(BaseStatement other)
        {
            return other is ExpressionWrapper exp
                   && exp.Equals(this);
        }

        public override string ToString()
        {
            return Expression.ToString();
        }
    }
}

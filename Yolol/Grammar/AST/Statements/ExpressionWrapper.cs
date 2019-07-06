using System;
using JetBrains.Annotations;
using Yolol.Execution;
using Yolol.Grammar.AST.Expressions;

namespace Yolol.Grammar.AST.Statements
{
    public class ExpressionWrapper
        : BaseStatement
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

        public override string ToString()
        {
            return Expression.ToString();
        }
    }
}

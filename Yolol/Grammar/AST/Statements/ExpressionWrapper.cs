using System;
using Yolol.Execution;
using Yolol.Grammar.AST.Expressions;

namespace Yolol.Grammar.AST.Statements
{
    public class ExpressionWrapper
        : BaseStatement
    {
        private readonly BaseExpression _expr;

        public ExpressionWrapper(BaseExpression expression)
        {
            _expr = expression ?? throw new ArgumentNullException(nameof(expression));
        }

        public override ExecutionResult Evaluate(MachineState state)
        {
            _expr.Evaluate(state);

            return new ExecutionResult();
        }

        public override string ToString()
        {
            return _expr.ToString();
        }
    }
}

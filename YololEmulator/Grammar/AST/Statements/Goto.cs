using System;
using YololEmulator.Execution;
using YololEmulator.Grammar.AST.Expressions;

namespace YololEmulator.Grammar.AST.Statements
{
    public class Goto
        : BaseStatement
    {
        private readonly BaseExpression _destination;

        public Goto(BaseExpression destination)
        {
            _destination = destination;
        }

        public override ExecutionResult Evaluate(MachineState state)
        {
            var dest = _destination.Evaluate(state);

            if (dest.Type != Execution.Type.Number)
                throw new ExecutionError($"Attempted to goto to a value of type `{dest.Type}`");

            var num = Math.Truncate(dest.Number);
            return new ExecutionResult((int)Math.Truncate(dest.Number));
        }
    }
}

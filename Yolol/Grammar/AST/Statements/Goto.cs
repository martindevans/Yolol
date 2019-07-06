using System;
using Yolol.Execution;
using Yolol.Grammar.AST.Expressions;

using Type = Yolol.Execution.Type;

namespace Yolol.Grammar.AST.Statements
{
    public class Goto
        : BaseStatement
    {
        public override bool CanRuntimeError => true;

        public BaseExpression Destination { get; }

        public Goto(BaseExpression destination)
        {
            Destination = destination;
        }

        public override ExecutionResult Evaluate(MachineState state)
        {
            var dest = Destination.Evaluate(state);

            if (dest.Type != Type.Number)
                throw new ExecutionException($"Attempted to goto to a value of type `{dest.Type}`");

            var line = Math.Clamp((int)dest.Number.Value, 1, 20);
            return new ExecutionResult(line);
        }

        public override string ToString()
        {
            return $"goto {Destination}";
        }
    }
}

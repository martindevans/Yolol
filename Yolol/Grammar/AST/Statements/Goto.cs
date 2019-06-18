using Yolol.Execution;
using Yolol.Grammar.AST.Expressions;

namespace Yolol.Grammar.AST.Statements
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

            if (dest.Type != Type.Number)
                throw new ExecutionException($"Attempted to goto to a value of type `{dest.Type}`");

            return new ExecutionResult((int)dest.Number.Value);
        }

        public override string ToString()
        {
            return $"goto {_destination}";
        }
    }
}

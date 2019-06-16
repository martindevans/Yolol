using System.Collections.Generic;
using YololEmulator.Execution;

namespace YololEmulator.Grammar.AST.Statements
{
    public class Line
    {
        public Line(IReadOnlyList<BaseStatement> statements)
        {
            Statements = statements;
        }

        public IReadOnlyList<BaseStatement> Statements { get; }

        public int Evaluate(int pc, MachineState state)
        {
            foreach (var statement in Statements)
            {
                var r = statement.Evaluate(state);
                switch (r.Type)
                {
                    case ExecutionResultType.Error:
                        throw new ExecutionException(r.ErrorMessage);

                    case ExecutionResultType.Goto:
                        return r.GotoLine - 1;

                    case ExecutionResultType.None:
                        continue;

                    default:
                        throw new ExecutionException($"Unknown ExecutionResult `{r.Type}`");

                }
            }

            return pc + 1;
        }
    }
}

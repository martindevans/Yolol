using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Statements
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
                    case ExecutionResultType.Goto:
                        return r.GotoLine - 1;

                    case ExecutionResultType.None:
                        continue;

                    //ncrunch: no coverage start
                    default:
                        throw new ExecutionException($"Unknown ExecutionResult `{r.Type}`");
                    //ncrunch: no coverage end

                }
            }

            return pc + 1;
        }

        public override string ToString()
        {
            return string.Join(" ", Statements.Select(s => s.ToString()));
        }
    }
}

using JetBrains.Annotations;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Statements
{
    public class Line
    {
        [NotNull] public StatementList Statements { get; }

        public Line([NotNull] StatementList statements)
        {
            Statements = statements;
        }

        public int Evaluate(int pc, [NotNull] MachineState state)
        {
            var result = Statements.Evaluate(state);
            if (result.Type == ExecutionResultType.Goto)
                return result.GotoLine - 1;
            else
                return pc + 1;
        }

        [NotNull] public override string ToString()
        {
            return Statements.ToString();
        }
    }
}

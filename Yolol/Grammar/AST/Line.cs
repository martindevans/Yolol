using System;
using Yolol.Execution;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Grammar.AST
{
    public class Line
        : IEquatable<Line>
    {
        public StatementList Statements { get; }

        public Line(StatementList statements)
        {
            Statements = statements;
        }

        public int Evaluate(int pc, MachineState state)
        {
            var result = Statements.Evaluate(state);
            if (result.Type == ExecutionResultType.Goto)
                return result.GotoLine - 1;
            else
                return pc + 1;
        }

        public bool Equals(Line? other)
        {
            return other != null && other.Statements.Equals(Statements);
        }

        public override string ToString()
        {
            return Statements.ToString();
        }
    }
}

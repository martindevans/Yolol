using System;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Statements
{
    public class EmptyStatement
        : BaseStatement, IEquatable<EmptyStatement>
    {
        public override bool CanRuntimeError => false;

        public override ExecutionResult Evaluate(MachineState state)
        {
            return new ExecutionResult();
        }

        public bool Equals(EmptyStatement? other)
        {
            return other != null;
        }

        public override bool Equals(BaseStatement? other)
        {
            return Equals(other as EmptyStatement);
        }

        public override string ToString()
        {
            return "";
        }

        public override int GetHashCode()
        {
            return 19;
        }
    }
}

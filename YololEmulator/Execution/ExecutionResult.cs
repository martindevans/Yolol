using System;

namespace YololEmulator.Execution
{
    public class ExecutionResult
    {
        public ExecutionResultType Type { get; }

        private readonly int _gotoLine;
        public int GotoLine
        {
            get
            {
                //ncrunch: no coverage start
                if (Type != ExecutionResultType.Goto)
                    throw new InvalidCastException($"Attempted to access variable of type {Type} as a Error");
                //ncrunch: no coverage end

                return _gotoLine;
            }
        }

        public ExecutionResult(int gotoLine)
        {
            Type = ExecutionResultType.Goto;
            _gotoLine = gotoLine;
        }

        public ExecutionResult()
        {
            Type = ExecutionResultType.None;
        }
    }

    public enum ExecutionResultType
    {
        None,
        Goto,
    }
}

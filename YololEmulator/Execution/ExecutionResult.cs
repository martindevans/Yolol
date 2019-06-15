using System;

namespace YololEmulator.Execution
{
    public class ExecutionResult
    {
        public ExecutionResultType Type { get; }

        private readonly string _errorMessage;
        public string ErrorMessage
        {
            get
            {
                if (Type != ExecutionResultType.Error)
                    throw new InvalidCastException($"Attempted to access variable of type {Type} as a Error");
                return _errorMessage;
            }
        }

        private readonly int _gotoLine;
        public int GotoLine
        {
            get
            {
                if (Type != ExecutionResultType.Goto)
                    throw new InvalidCastException($"Attempted to access variable of type {Type} as a Error");
                return _gotoLine;
            }
        }

        public ExecutionResult(string errorMessage)
        {
            Type = ExecutionResultType.Error;
            _errorMessage = errorMessage;
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
        Error,
        Goto,
    }
}

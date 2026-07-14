using System;

namespace Yolol.Execution
{
    public readonly struct ExecutionResult
        : IEquatable<ExecutionResult>
    {
        public ExecutionResultType Type { get; }

        private readonly int _gotoLine;
        public int GotoLine
        {
            get
            {
                //ncrunch: no coverage start
                if (Type != ExecutionResultType.Goto)
                    throw new InvalidOperationException($"Attempted to access variable of type {Type} as a Error");
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

        #region equality
        public bool Equals(ExecutionResult other)
        {
            return _gotoLine == other._gotoLine && Type == other.Type;
        }

        public override bool Equals(object? obj)
        {
            return obj is ExecutionResult other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_gotoLine, (int)Type);
        }

        public static bool operator ==(ExecutionResult left, ExecutionResult right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ExecutionResult left, ExecutionResult right)
        {
            return !(left == right);
        }
        #endregion
    }

    public enum ExecutionResultType
    {
        None,
        Goto,
    }
}

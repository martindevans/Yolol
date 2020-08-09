namespace Yolol.Execution
{
    /// <summary>
    /// Represents a runtime error that is statically known to happen
    /// </summary>
    public readonly struct StaticError
    {
        private readonly string _message;

        public StaticError(string message)
        {
            _message = message;
        }

        public static implicit operator Value(StaticError s)
        {
            throw new ExecutionException(s._message);
        }
    }
}

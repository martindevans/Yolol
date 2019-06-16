using System;

namespace YololEmulator.Execution
{
    public class ExecutionException
        : Exception
    {
        public ExecutionException(string message)
            : base(message)
        {
            
        }
    }
}

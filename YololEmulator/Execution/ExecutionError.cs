using System;

namespace YololEmulator.Execution
{
    public class ExecutionError
        : Exception
    {
        public ExecutionError(string message)
            : base(message)
        {
            
        }
    }
}

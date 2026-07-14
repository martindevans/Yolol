using System;

namespace Yolol.Execution
{
    public class ExecutionException
        : Exception
    {
        public ExecutionException()
        {
        }
        
        public ExecutionException(string message)
            : base(message)
        {
        }

        public ExecutionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

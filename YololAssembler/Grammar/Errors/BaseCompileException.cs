using System;

namespace YololAssembler.Grammar.Errors
{
    public class BaseCompileException
        : Exception
    {
        public BaseCompileException(string message)
            : base(message)
        {
        }

        public override string ToString()
        {
            return Message;
        }
    }
}

using System;

namespace Yolol.Execution
{
    public class ThrowDeviceNetwork
        : IDeviceNetwork
    {
        public IVariable Get(string name)
        {
            throw new NullDeviceNetworkAccessException(name);
        }

        public class NullDeviceNetworkAccessException
            : NotSupportedException
        {
            public NullDeviceNetworkAccessException(string name)
                : base($"Attempted to access external field `{name}`")
            {
            }
        }
    }
}

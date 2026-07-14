using System;

namespace Yolol.Execution
{
    /// <summary>
    /// Throws a <see cref="NullDeviceNetworkAccessException"/> on every attempt to access an external variable
    /// </summary>
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

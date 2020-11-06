using System.Collections.Generic;
using Yolol.Grammar;

namespace Yolol.Execution
{
    public interface IDeviceNetwork
    {
        IVariable Get(string name);
    }
}

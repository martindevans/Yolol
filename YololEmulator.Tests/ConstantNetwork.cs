using System.Collections.Generic;
using Yolol.Execution;

namespace YololEmulator.Tests
{
    public class ConstantNetwork
        : IDeviceNetwork
    {
        private readonly Dictionary<string, IVariable> _variables = new Dictionary<string, IVariable>();

        public ConstantNetwork(params KeyValuePair<string, Value>[] values)
        {
            foreach (var (key, value) in values)
                _variables.Add(key, new Variable { Value = value });
        }

        public IVariable Get(string name)
        {
            name = name.ToLowerInvariant();

            if (!_variables.TryGetValue(name, out var v))
                _variables.Add(name, v = new Variable { Value = new Value(0) });
            return v;
        }
    }
}

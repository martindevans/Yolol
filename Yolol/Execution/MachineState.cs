using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Yolol.Grammar;

namespace Yolol.Execution
{
    public class MachineState
        : IEnumerable<KeyValuePair<string, IVariable>>
    {
        public ushort MaxLineNumber { get; }

        private readonly IDeviceNetwork _network;

        private readonly Dictionary<string, IVariable> _variables = new Dictionary<string, IVariable>();

        public MachineState([NotNull] IDeviceNetwork network, ushort maxLineNumber = 20)
        {
            MaxLineNumber = maxLineNumber;
            _network = network ?? throw new ArgumentNullException(nameof(network));
        }

        public IVariable GetVariable(string name)
        {
            name = name.ToLowerInvariant();

            if (name.StartsWith(":"))
            {
                return _network.Get(name.Substring(1));
            }
            else
            {
                if (!_variables.TryGetValue(name, out var v))
                    _variables.Add(name, v = new Variable { Value = new Value(0) });
                return v;
            }
        }

        [NotNull] internal IVariable GetVariable([NotNull] VariableName name) => GetVariable(name.Name);

        public IEnumerator<KeyValuePair<string, IVariable>> GetEnumerator()
        {
            return _variables.GetEnumerator();
        }

        //ncrunch: no coverage start
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_variables).GetEnumerator();
        }
        //ncrunch: no coverage end
    }
}

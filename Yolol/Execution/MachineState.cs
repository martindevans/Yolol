using System;
using System.Collections;
using System.Collections.Generic;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Execution
{
    public class MachineState
        : IEnumerable<KeyValuePair<string, IVariable>>
    {
        private readonly IDeviceNetwork _network;

        private readonly Dictionary<string, IVariable> _variables = new Dictionary<string, IVariable>();

        public MachineState(IDeviceNetwork network)
        {
            _network = network ?? throw new ArgumentNullException(nameof(network));
        }

        public IVariable Get(string name)
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

        internal IVariable Get(VariableName name) => Get(name.Name);

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

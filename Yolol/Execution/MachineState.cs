using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Yolol.Grammar;

namespace Yolol.Execution
{
    public class MachineState
        : IEnumerable<KeyValuePair<string, IVariable>>
    {
        public ushort MaxLineNumber { get; }
        public int MaxStringLength { get; }

        private readonly IDeviceNetwork _network;

        private readonly Dictionary<string, IVariable> _variables = new();

        public MachineState(IDeviceNetwork network, ushort maxLineNumber = 20, int maxStringLength = 1024)
        {
            MaxLineNumber = maxLineNumber;
            MaxStringLength = maxStringLength;

            _network = network ?? throw new ArgumentNullException(nameof(network));
        }

        public IVariable GetVariable(string name)
        {
            return GetVariable(new VariableName(name));
        }

        internal IVariable GetVariable(VariableName name)
        {
            if (name.IsExternal)
                return _network.Get(name.PureName);
            
            if (!_variables.TryGetValue(name.PureName, out var v))
                _variables.Add(name.PureName, v = new Variable { Value = new Value((Number)0) });
            return v;
        }

        public IEnumerator<KeyValuePair<string, IVariable>> GetEnumerator()
        {
            return _variables.GetEnumerator();
        }

        [ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_variables).GetEnumerator();
        }
    }
}

﻿using System;
using System.Collections;
using System.Collections.Generic;
using Yolol.Grammar;

namespace Yolol.Execution
{
    public class MachineState
        : IEnumerable<KeyValuePair<string, IVariable>>
    {
        public ushort MaxLineNumber { get; }
        public int MaxStringLength { get; }

        private readonly IDeviceNetwork _network;

        private readonly Dictionary<string, IVariable> _variables = new Dictionary<string, IVariable>();

        public MachineState(IDeviceNetwork network, ushort maxLineNumber = 20, int maxStringLength = 1024)
        {
            MaxLineNumber = maxLineNumber;
            MaxStringLength = maxStringLength;

            _network = network ?? throw new ArgumentNullException(nameof(network));
        }

        public IVariable GetVariable(string name)
        {
            name = name.ToLowerInvariant();

            if (name.StartsWith(":", StringComparison.Ordinal))
            {
                return _network.Get(name[1..]);
            }
            else
            {
                if (!_variables.TryGetValue(name, out var v))
                    _variables.Add(name, v = new Variable { Value = new Value((Number)0) });
                return v;
            }
        }

        internal IVariable GetVariable(VariableName name) => GetVariable(name.Name);

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

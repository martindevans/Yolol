using System;
using System.Collections.Generic;
using YololEmulator.Grammar.AST.Statements;

namespace YololEmulator.Execution
{
    public class MachineState
    {
        private readonly Dictionary<string, Variable> _variables = new Dictionary<string, Variable>();

        public Variable Get(string name)
        {
            name = name.ToLowerInvariant();

            if (!_variables.TryGetValue(name, out var v))
                _variables.Add(name, v = new Variable(name, name.StartsWith(':')) { Value = new Value(0) });
            return v;
        }

        internal Variable Get(VariableName name) => Get(name.Name);

        public void Print(string prefix)
        {
            foreach (var (key, value) in _variables)
            {
                Console.WriteLine($"{prefix}{key} = {value}");
            }
        }
    }
}

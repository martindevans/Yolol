using System;
using System.Collections.Generic;
using Yolol.Execution;

namespace YololEmulator.Network
{
    public class ConsoleInputDeviceNetwork
        : IDeviceNetwork
    {
        private readonly bool _save;
        private readonly Dictionary<string, IVariable> _saved = new Dictionary<string, IVariable>();

        public ConsoleInputDeviceNetwork(bool save)
        {
            _save = save;
        }

        public IVariable Get(string name)
        {
            if (_save)
            {
                if (!_saved.TryGetValue(name, out var v))
                {
                    v = new ConsoleInputVariable(name);
                    _saved.Add(name, v);
                }

                return v;
            }
            else
                return new ConsoleInputVariable(name);
        }

        private class ConsoleInputVariable
            : IVariable
        {
            private readonly string _name;

            private Value? _savedValue;
            public Value Value
            {
                get
                {
                    if (_savedValue.HasValue)
                        return _savedValue.Value;

                    var c = Console.ForegroundColor;
                    try
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        while (true)
                        {
                            Console.WriteLine($" -> Enter value for `:{_name}`");

                            var value = Console.ReadLine();
                            if (string.IsNullOrWhiteSpace(value))
                                continue;

                            if (value.StartsWith('"') && value.EndsWith('"'))
                            {
                                _savedValue = new Value(value[1..^1]);
                                return _savedValue.Value;
                            }

                            if (decimal.TryParse(value, out var result))
                            {
                                _savedValue = new Value(result);
                                return _savedValue.Value;
                            }

                            Console.WriteLine(" -> Could not parse as a string or a number!");
                        }
                    }
                    finally
                    {
                        Console.ForegroundColor = c;
                    }
                }
                set
                {
                    _savedValue = value;

                    var c = Console.ForegroundColor;
                    try
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($" -> Set `:{_name}` to {value}");
                    }
                    finally
                    {
                        Console.ForegroundColor = c;
                    }
                }
            }

            public ConsoleInputVariable(string name)
            {
                _name = name;
            }
        }
    }
}

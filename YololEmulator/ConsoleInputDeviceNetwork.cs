using System;
using Yolol.Execution;

namespace YololEmulator
{
    public class ConsoleInputDeviceNetwork
        : IDeviceNetwork
    {
        public IVariable Get(string name)
        {
            return new ConsoleInputVariable(name);
        }

        private class ConsoleInputVariable
            : IVariable
        {
            private readonly string _name;

            public Value Value
            {
                get
                {
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
                                return new Value(value.Substring(1, value.Length - 2));

                            if (decimal.TryParse(value, out var result))
                                return new Value(result);

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

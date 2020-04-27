using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Yolol.Cylon.JSON;
using Yolol.Execution;
using Yolol.Grammar.AST;

namespace YololEmulator.Tests.Ladder
{
    public static class Generator
    {
        private class Data
        {
            [JsonProperty("in")]
            public Dictionary<string, Value>[] In { get; set; }

            [JsonProperty("out")]
            public Dictionary<string, Value>[] Out { get; set; }
        }

        private class DeviceNetwork
            : IDeviceNetwork
        {
            private readonly Dictionary<string, IVariable> _saved = new Dictionary<string, IVariable>();

            public IVariable Get(string name)
            {
                if (!_saved.TryGetValue(name, out var v))
                {
                    v = new Variable { Value = 0 };
                    _saved.Add(name, v);
                }

                return v;
            }
        }

        public static void YololLadderGenerator(List<Dictionary<string, Value>> input, List<Dictionary<string, Value>> output)
        {
            var d = new Data() {
                In = input.ToArray(),
                Out = output.ToArray()
            };

            Console.WriteLine(JsonConvert.SerializeObject(d, new YololValueConverter()));
        }

        public static IEnumerable<(IReadOnlyDictionary<string, Value>, IReadOnlyDictionary<string, Value>)> RunYololGenerator(Func<Dictionary<string, Value>> generateInput, int count, Program ast, params string[] outputs)
        {
            // Get the variable which the program uses to indicate it is ready to move to the next round
            var state = new MachineState(new DeviceNetwork());
            var done = state.GetVariable($":done");

            // Run through test cases one by one
            var pc = 0;
            for (var i = 0; i < count; i++)
            {
                // Set inputs
                var inputs = generateInput();
                foreach (var (key, value) in inputs)
                    state.GetVariable($":{key}").Value = value;

                // Clear completion indicator
                done.Value = 0;

                // Run lines until completion indicator is set or execution time limit is exceeded
                while (!done.Value.ToBool())
                {
                    try
                    {
                        // If line if blank, just move to the next line
                        if (pc >= ast.Lines.Count)
                            pc++;
                        else
                            pc = ast.Lines[pc].Evaluate(pc, state);
                    }
                    catch (ExecutionException)
                    {
                        pc++;
                    }

                    // loop around if program counter goes over max
                    if (pc >= 20)
                        pc = 0;
                }

                // Check outputs
                var output = new Dictionary<string, Value>();
                foreach (var key in outputs)
                {
                    var v = state.GetVariable($":{key}");
                    output.Add(key, v.Value);
                }
                yield return (inputs, output);
            }
        }
    }
}

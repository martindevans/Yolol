using System;
using System.Collections.Generic;
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
            [JsonProperty("shuffle")]
            public bool Shuffle { get; }

            [JsonProperty("mode")]
            public ScoreMode Mode { get; }

            [JsonProperty("chip")]
            public YololChip Chip { get; }

            [JsonProperty("in")]
            public Dictionary<string, Value>[] In { get; }

            [JsonProperty("out")]
            public Dictionary<string, Value>[] Out { get; }

            public Data(bool shuffle, ScoreMode mode, YololChip chip, Dictionary<string, Value>[] inputs, Dictionary<string, Value>[] outputs)
            {
                Shuffle = shuffle;
                Mode = mode;
                Chip = chip;
                In = inputs;
                Out = outputs;
            }
        }

        private class DeviceNetwork
            : IDeviceNetwork
        {
            private readonly Dictionary<string, IVariable> _saved = new();

            public IVariable Get(string name)
            {
                if (!_saved.TryGetValue(name, out var v))
                {
                    v = new Variable { Value = (Value)0 };
                    _saved.Add(name, v);
                }

                return v;
            }
        }

        public enum YololChip
        {
            Unknown = 0,

            Basic = 1,
            Advanced = 2,
            Professional = 3,
        }

        public enum ScoreMode
        {
            Unknown = 0,

            BasicScoring = 1,
            Approximate = 2,
        }

        public static void YololLadderGenerator(List<Dictionary<string, Value>> input, List<Dictionary<string, Value>> output, bool shuffle = true, ScoreMode mode = ScoreMode.BasicScoring, YololChip chip = YololChip.Professional)
        {
            var d = new Data(shuffle, mode, chip, input.ToArray(), output.ToArray());

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
                done.Value = (Value)0;

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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Yolol.Execution;
using Yolol.Grammar;
using Yolol.Grammar.AST;

using Type = Yolol.Execution.Type;

namespace Yolol.Analysis.Fuzzer
{
    public class QuickFuzz
    {
        private readonly Dictionary<VariableName, Type> _hints;

        public QuickFuzz([NotNull] params (VariableName, Type)[] typeHints)
        {
            _hints = typeHints.ToDictionary(a => a.Item1, a => a.Item2);
        }

        [NotNull] public async Task<IFuzzResult> Fuzz([NotNull] Program program, int runs = 250, int iterations = 50)
        {
            // Start as many tasks as necessary
            var work = new Task<FuzzResultItem>[runs];
            for (var i = 0; i < runs; i++)
                work[i] = Start(program, i, iterations);

            // Wait for them all to finish
            await Task.WhenAll(work);

            // Return a result aggregating together all of the results
            return new FuzzResult(work.Select(a => (IFuzzResultItem)a.Result).ToArray());
        }

        private Task<FuzzResultItem> Start(Program program, int index, int maxIters)
        {
            return Task.Run(() => {

                // Set up a network which generates random values when asked and stores all values when set
                var rngNetwork = new RandomNetwork(index, _hints);

                // Run the program until it terminates - tries to execute a line out of bounds, or runs out of it's iters limit
                var state = new MachineState(rngNetwork);
                var pc = 0;
                while (pc < program.Lines.Count && pc >= 0 && rngNetwork.Iterations < maxIters)
                {
                    rngNetwork.NextIteration();

                    try
                    {
                        pc = program.Lines[pc].Evaluate(pc, state);
                    }
                    catch (ExecutionException)
                    {
                        pc++;
                    }
                }

                // Save this run
                return new FuzzResultItem(index, rngNetwork.Iterations, rngNetwork.Sets, rngNetwork.Gets);
            });
        }

        private class RandomNetwork
            : IDeviceNetwork
        {
            private readonly int _seed;
            private readonly Dictionary<VariableName, Type> _hints;

            private readonly List<(string, int, Value)> _sets = new List<(string, int, Value)>();
            private readonly List<(string, int, Value)> _gets = new List<(string, int, Value)>();

            public RandomNetwork(int seed, Dictionary<VariableName, Type> hints)
            {
                _seed = seed;
                _hints = hints;
            }

            public int Iterations { get; private set; }

            public IReadOnlyList<(string, int, Value)> Sets => _sets;
            public IReadOnlyList<(string, int, Value)> Gets => _gets;

            public void NextIteration()
            {
                Iterations++;
            }

            public IVariable Get(string name)
            {
                return new RecordingVariable(name, this);
            }

            private void SetValue(string name, Value value)
            {
                _sets.Add((name, Iterations, value));
            }

            private Value GetValue([NotNull] string name)
            {
                Value GetInner()
                {
                    const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

                    // Create a new RNG based on the name and the iteration count. This means that if a program only differs in reads it remains in sync.
                    var rng = new Random(unchecked(name.GetHashCode() + Iterations + _seed));

                    // Generate 4 possible values: number, integer, alphanumeric string and boolean
                    var a = new Value((decimal)rng.NextDouble());
                    var b = new Value(rng.Next(int.MinValue, int.MaxValue));
                    var c = new string(Enumerable.Repeat(chars, rng.Next(5, 100)).Select(s => s[rng.Next(s.Length)]).ToArray());
                    var d = rng.Next(0, 2);

                    var r = rng.NextDouble();

                    // If we have a type hint for this value return a value of the correct type
                    if (_hints.TryGetValue(new VariableName(":" + name), out var hint))
                    {
                        if (hint == Type.Number)
                        {
                            if (r < 0.33f)
                                return a;
                            if (r < 0.66)
                                return b;
                            return c;
                        }

                        if (hint == Type.String)
                            return c;
                    }

                    // Select a random value to return
                    if (r < 0.25f)
                        return a;
                    if (r < 0.5f)
                        return b;
                    if (r < 0.75f)
                        return c;
                    return d;
                }

                var v = GetInner();
                _gets.Add((name, Iterations, v));
                return v;
            }

            private class RecordingVariable
                : IVariable
            {
                private readonly string _name;
                private readonly RandomNetwork _parent;

                public Value Value
                {
                    get => _parent.GetValue(_name);
                    set => _parent.SetValue(_name, value);
                }

                public RecordingVariable(string name, RandomNetwork parent)
                {
                    _name = name;
                    _parent = parent;
                }
            }
        }

        private class FuzzResultItem
            : IFuzzResultItem
        {
            public int Index { get; }

            public int IterCount { get; }

            public IReadOnlyList<(string, int, Value)> Sets { get; }

            public IReadOnlyList<(string, int, Value)> Gets { get; }

            public FuzzResultItem(int index, int iterCount, IReadOnlyList<(string, int, Value)> sets, IReadOnlyList<(string, int, Value)> gets)
            {
                Index = index;
                IterCount = iterCount;
                Sets = sets;
                Gets = gets;
            }

            public bool Equals(IFuzzResultItem other)
            {
                if (other == null || Index != other.Index || IterCount != other.IterCount || Sets.Count != other.Sets.Count)
                    return false;

                for (var i = 0; i < Sets.Count; i++)
                    if (!Sets[i].Equals(other.Sets[i]))
                        return false;

                return true;
            }

            public override string ToString()
            {
                return $"idx:{Index} its:{IterCount} vars:{string.Join(",", Sets)}";
            }
        }

        private class FuzzResult
            : IFuzzResult
        {
            private readonly IFuzzResultItem[] _results;

            public FuzzResult(IFuzzResultItem[] results)
            {
                _results = results;
            }

            public int Count => _results.Length;

            public IFuzzResultItem this[int index] => _results.Single(a => a.Index == index);

            public bool Equals(IFuzzResult other)
            {
                if (!(other is FuzzResult r))
                    return false;

                for (var i = 0; i < _results.Length; i++)
                    if (!this[i].Equals(r[i]))
                        return false;

                return true;
            }

            public IEnumerator<IFuzzResultItem> GetEnumerator()
            {
                return ((IEnumerable<IFuzzResultItem>)_results).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}

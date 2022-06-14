using System;
using System.Collections.Generic;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    public abstract class BaseGenerator
    {
        protected int Count { get; private set; }

        protected bool RepeatedFailed = false;

        protected void Run(int seed, int count, bool shuffle, Generator.ScoreMode mode, Generator.YololChip chip = Generator.YololChip.Professional)
        {
            Count = count;
            var rng = new Random(seed);

            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            Setup(input, output);

            for (var i = 0; i < count; i++)
            {
                var ii = new Dictionary<string, Value>();
                var io = new Dictionary<string, Value>();
                if (GenerateCase(rng, i, ii, io))
                {
                    input.Add(ii);
                    output.Add(io);
                }
                else if (RepeatedFailed)
                {
                    i--;
                }
            }

            Finalise(input, output);

            Generator.YololLadderGenerator(input, output, shuffle, mode, chip);
        }

        protected virtual void Finalise(List<Dictionary<string, Value>> input, List<Dictionary<string, Value>> output)
        {
        }

        protected virtual void Setup(List<Dictionary<string, Value>> inputs, List<Dictionary<string, Value>> outputs)
        {
        }

        protected abstract bool GenerateCase(Random random, int index, Dictionary<string, Value> inputs, Dictionary<string, Value> outputs);
    }
}

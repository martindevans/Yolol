using System;
using System.Collections.Generic;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    public abstract class BaseGenerator
    {
        protected void Run(int seed, int count, bool shuffle, Generator.ScoreMode mode, Generator.YololChip chip = Generator.YololChip.Professional)
        {
            var rng = new Random(seed);

            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            for (var i = 0; i < count; i++)
            {
                var ii = new Dictionary<string, Value>();
                var io = new Dictionary<string, Value>();
                if (GenerateCase(rng, i, ii, io))
                {
                    input.Add(ii);
                    output.Add(io);
                }
            }

            Generator.YololLadderGenerator(input, output, shuffle, mode, chip);
        }

        protected abstract bool GenerateCase(Random random, int index, Dictionary<string, Value> inputs, Dictionary<string, Value> outputs);
    }
}

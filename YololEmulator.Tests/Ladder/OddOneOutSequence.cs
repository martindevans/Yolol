using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;
using System.Linq;
using MoreLinq.Extensions;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class OddOneOutSequence
    {
        [TestMethod]
        public void GenerateOddOneOutSequence()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            var rng = new Random(453689);
            for (var j = 0; j < 20000; j++)
            {
                var (q, a) = rng.NextDouble() < 0.5
                           ? GenerateEvens(rng)
                           : GenerateOdds(rng);

                input.Add(new Dictionary<string, Value> {{"i", q}});
                output.Add(new Dictionary<string, Value> {{"o", a}});
            }

            Generator.YololLadderGenerator(input, output);
        }

        private static int RandomEven(Random rand)
        {
            while (true)
            {
                var r = rand.Next(0, 10);
                if (r % 2 == 0)
                    return r;
            }
        }

        private static int RandomOdd(Random rand)
        {
            while (true)
            {
                var r = rand.Next(0, 10);
                if (r % 2 == 1)
                    return r;
            }
        }

        private static (Number seq, Number ans) GenerateEvens(Random rand)
        {
            var extra = RandomOdd(rand);
            var sequence = Enumerable.Range(0, 5)
                .Select(_ => RandomEven(rand))
                .ToList();
            sequence.Add(extra);

            var str = string.Join("", sequence.Shuffle(rand));
            var num = int.Parse(str);

            return ((Number)num, (Number)extra);
        }

        private static (Number seq, Number ans) GenerateOdds(Random rand)
        {
            var extra = RandomEven(rand);
            var sequence = Enumerable.Range(0, 5)
                .Select(_ => RandomOdd(rand))
                .ToList();
            sequence.Add(extra);

            var str = string.Join("", sequence.Shuffle(rand));
            var num = int.Parse(str);

            return ((Number)num, (Number)extra);
        }
    }
}
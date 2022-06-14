using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoreLinq;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class PermutationCount
        : BaseGenerator
    {
        private static string RandomString(Random rng, int minLength, int maxLength)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz";

            var length = rng.Next(minLength, maxLength + 1);
            var order = Enumerable.Repeat(chars, length)
                .Select(s => chars[rng.Next(chars.Length)])
                .ToArray();

            order[0] = char.ToUpper(order[0]);

            return string.Join("", order);
        }

        [TestMethod]
        public void Generate()
        {
            Run(56432454, 9999, true, Generator.ScoreMode.BasicScoring);
        }

        protected override bool GenerateCase(Random random, int index, Dictionary<string, Value> inputs, Dictionary<string, Value> outputs)
        {
            var str = RandomString(random, 3, 7);
            var count = str.Permutations().Select(a => string.Concat(a)).Distinct().Count();
            if (count > 8000)
                return false;

            inputs.Add("i", str);
            outputs.Add("o", (Value)count);

            return true;
        }
    }
}

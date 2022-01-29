using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class ZigZagText
        : BaseGenerator
    {
        [TestMethod]
        public void Generate()
        {
            Run(346345, 50000, true, Generator.ScoreMode.BasicScoring);
        }

        private string[] _examples = new[]
        {
            "HelloCylon",
            "ZigZag Strings",
            "Are always more than 10 chars",
            "and less than 30",
            "Good luck!"
        };

        protected override bool GenerateCase(Random random, int index, Dictionary<string, Value> inputs, Dictionary<string, Value> outputs)
        {
            var str = RandomString(random, 11, 29);
            if (index < 5)
                str = _examples[index];

            var l0 = new List<char>();
            var l1 = new List<char>();
            var l2 = new List<char>();

            foreach (var character in str.Chunk(4))
            {
                if (character.Length > 0)
                    l0.Add(character[0]);
                if (character.Length > 1)
                    l1.Add(character[1]);
                if (character.Length > 2)
                    l2.Add(character[2]);
                if (character.Length > 3)
                    l1.Add(character[3]);
            }

            var output = string.Join("", l0) + string.Join("", l1) + string.Join("", l2);

            inputs.Add("i", str);
            outputs.Add("o", output);
            
            return true;
        }

        private static string RandomString(Random rng, int minLength, int maxLength)
        {
            const string chars = " ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!?";

            var length = rng.Next(minLength, maxLength);
            var order = Enumerable.Repeat(chars, length)
                .Select(s => chars[rng.Next(chars.Length)])
                .ToArray();

            return string.Join("", order);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoreLinq;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class SpotTheDifference
        : BaseGenerator
    {
        [TestMethod]
        public void Generate()
        {
            Run(346345, 25000, true, Generator.ScoreMode.BasicScoring);
        }

        protected override bool GenerateCase(Random random, int index, Dictionary<string, Value> inputs, Dictionary<string, Value> outputs)
        {
            var word = RandomString(random, 5, 9);
            var a = RandomString(random, word.Length, word.Length);
            var b = ChangeOneChar(random, ChangeOneChar(random, word));
            var c = ChangeOneChar(random, word);

            // Generate 3 randomly mutated words
            var diffs = new[]
            {
                (a, StringDifference(word, a)),
                (b, StringDifference(word, b)),
                (c, StringDifference(word, c))
            };

            // Don't include this test if there is more than one answer
            if (diffs.Count(x => x.Item2 == 1) != 1)
                return false;
            var answerDiff = diffs.Single(x => x.Item2 == 1);

            // Shuffle the words so that the right answer (c) isn't always last
            var words = new[] { a, b, c }.Shuffle(random).ToList();

            // Find the index of the word with a diff of 1
            var answer = -1;
            for (var i = 0; i < words.Count; i++)
            {
                if (answerDiff.Item1 != words[i])
                    continue;

                if (answer != -1)
                    return false;
                answer = i;
            }

            if (answer == -1)
                return false;

            // outputs the question
            inputs.Add("w", word);
            inputs.Add("a", words[0]);
            inputs.Add("b", words[1]);
            inputs.Add("c", words[2]);

            // And the answer
            outputs.Add("o", (Value)answer);

            return true;
        }

        private static int StringDifference(string a, string b)
        {
            return a
                .Zip(b, (x, y) => x == y)
                .Count(z => !z);
        }

        private static string ChangeOneChar(Random random, string word)
        {
            var idx = random.Next(word.Length);
            var arr = word.ToCharArray();
            arr[idx] = RandomString(random, 1, 1)[0];
            return new string(arr);
        }

        private static string RandomString(Random rng, int minLength, int maxLength)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz";

            var length = rng.Next(minLength, maxLength);
            var order = Enumerable.Repeat(chars, length)
                .Select(_ => chars[rng.Next(chars.Length)])
                .ToArray();

            return string.Join("", order);
        }
    }
}

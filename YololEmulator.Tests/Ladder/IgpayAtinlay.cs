using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class IgpayAtinlay
        : BaseGenerator
    {
        private readonly string[] _initialCases = {
            "Hello Cylon",
            "Translate the inputs",
            "Into pig latin",
            "Good luck",
            "Oodgay klucay",
        };

        [TestMethod]
        public void Generate()
        {
            Run(86341, 25000, true, Generator.ScoreMode.BasicScoring);
        }

        protected override bool GenerateCase(Random random, int index, Dictionary<string, Value> inputs, Dictionary<string, Value> outputs)
        {
            IReadOnlyList<string> words;
            if (index < _initialCases.Length)
                words = _initialCases[index].Split(" ");
            else
                words = Enumerable.Range(0, random.Next(10, 30)).Select(i => RandomString(random, 5, 10, i == 0)).ToList();


            inputs.Add("i", string.Join(" ", words) + ".");

            var pigged = words.Select(PigLatin).ToList();
            outputs.Add("o", string.Join(" ", pigged) + ".");

            return true;
        }

        private static string RandomString(Random rng, int minLength, int maxLength, bool upperCase)
        {
            const string lower = "abcdefghijklmnopqrstuvwxyz";

            var length = rng.Next(minLength, maxLength);
            var chars = Enumerable.Repeat(lower, length)
                                  .Select(s => lower[rng.Next(lower.Length)])
                                  .ToArray();

            if (upperCase)
                chars[0] = char.ToUpperInvariant(chars[0]);

            return string.Join("", chars);
        }

        private static string PigLatin(string word)
        {
            // move the first letter of each word to the end of the word, then add "ay"
            var w = word[^1] + word[..^1].ToLowerInvariant() + "ay";

            if (char.IsUpper(word[0]))
                w = char.ToUpperInvariant(w[0]) + w[1..];

            return w;

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoreLinq;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class StringPatterns
        : BaseGenerator
    {
        private readonly string[] _words = new[]
        {
            "Cylon", "Yolol", "IL", "Referee", "Toaster", "ISAN", "Starbase", "Vasama", "Eos"
        };

        [TestMethod]
        public void Generate()
        {
            Run(67343, 15000, true, Generator.ScoreMode.BasicScoring);
        }

        protected override bool GenerateCase(Random random, int index, Dictionary<string, Value> inputs, Dictionary<string, Value> outputs)
        {
            // Choose a base offset to make sure indices aren't the same every time
            var offset = random.Next(15);

            // Pick some words and generate the pattern
            var words = new List<string>();
            var pattern = new List<char>();
            var count = random.Next(1, 8);
            for (var i = 0; i < count; i++)
            {
                var idx = random.Next(_words.Length);
                words.Add(_words[idx]);
                pattern.Add((char)(idx + 65 + offset));
            }
            
            // Maybe corrupt the pattern
            if (random.NextDouble() < 0.5)
                Corrupt(pattern, random);

            // Check if it matches
            var match = Check(words, pattern);

            inputs.Add("s", string.Join(" ", words));
            inputs.Add("p", string.Join("", pattern));
            outputs.Add("o", (Number)match);
            
            return true;
        }

        private static bool Check(IReadOnlyList<string> words, IReadOnlyList<char> pattern)
        {
            if (words.Count != pattern.Count)
                return false;

            var charToWord = new Dictionary<char, string>();
            var wordToChar = new Dictionary<string, char>();

            for (int i = 0; i < words.Count; i++)
            {
                var w = words[i];
                var c = pattern[i];

                // Check if the word is already mapped to a different character
                if (wordToChar.TryGetValue(w, out var c1))
                    if (c1 != c)
                        return false;


                // Check if the char is already mapped to a different word
                if (charToWord.TryGetValue(c, out var w1))
                    if (w1 != w)
                        return false;

                // Store the mapping discovered
                wordToChar[w] = c;
                charToWord[c] = w;
            }

            return true;
        }

        private static void Corrupt(List<char> pattern, Random random)
        {
            var idx = random.Next(pattern.Count);

            switch (random.Next(4))
            {
                case 0:
                    fallback:
                    pattern.RemoveAt(idx);
                    return;

                case 1:
                    if (pattern.SequenceEqual(pattern.AsEnumerable().Reverse()))
                        goto fallback;
                    pattern.Reverse();
                    return;

                case 2:
                    pattern.Insert(random.Next(pattern.Count), (char)random.Next(65, 90));
                    return;

                case 3:
                    var s = pattern.Shuffle().ToList();
                    pattern.Clear();
                    pattern.AddRange(s);
                    return;
            }
        }
    }
}
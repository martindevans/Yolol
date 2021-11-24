using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoreLinq.Extensions;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class Clambserd
        : BaseGenerator
    {
        private readonly string[] _initialCases = {
            "HelloCylon",
            "FindTheScrambledInput",
            "FromFiveOptions",
            "GoodLuck",
            "GLkooduc",
        };

        [TestMethod]
        public void Generate()
        {
            Run(64545, 50000, true, Generator.ScoreMode.BasicScoring);
        }

        protected override bool GenerateCase(Random random, int index, Dictionary<string, Value> inputs, Dictionary<string, Value> outputs)
        {
            string word;
            if (index < _initialCases.Length)
                word = _initialCases[index];
            else
                word = RandomString(random, 5, 20);

            var result = Scramble(random, word);
            if (result == null)
                return false;

            var (a, b, c, d, idx) = result.Value;

            inputs.Add("i", word);
            inputs.Add("a", a);
            inputs.Add("b", b);
            inputs.Add("c", c);
            inputs.Add("d", d);

            outputs.Add("o", (Number)idx);

            return true;
        }

        private static string RandomString(Random rng, int minLength, int maxLength)
        {
            const string lower = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

            var length = rng.Next(minLength, maxLength);
            var chars = Enumerable.Repeat(lower, length)
                                  .Select(s => lower[rng.Next(lower.Length)])
                                  .ToArray();

            return string.Join("", chars);
        }

        private static (string, string, string, string, int)? Scramble(Random rng, string word)
        {
            var scrambled = new string(word.Shuffle().ToArray());

            var m1 = new string(Mutate1(rng, scrambled.ToList()).ToArray());
            var m2 = new string(Mutate1(rng, scrambled.ToList()).ToArray());
            var m3 = new string(Mutate2(rng, scrambled.ToList()).ToArray());

            // Check that none of the alternatives are accidentally permutations of the original input
            if (ArePermutation(m1, word) || ArePermutation(m2, word) || ArePermutation(m3, word))
                return null;

            var arr = new[] { scrambled, m1, m2, m3 }.Shuffle().ToArray();
            return (arr[0], arr[1], arr[2], arr[3], Array.IndexOf(arr, scrambled));
            
        }

        private static List<char> Mutate1(Random rng, List<char> s)
        {
            // Replace some characters with some other characters
            var count = rng.Next(0, s.Count / 2);
            for (var i = 0; i < count; i++)
                ReplaceChar(rng, s);

            return s;
        }

        private static List<char> Mutate2(Random rng, List<char> s)
        {
            // Remove some characters
            var count = rng.Next(0, s.Count / 4);
            for (int i = 0; i < count; i++)
                s.RemoveAt(rng.Next(0, s.Count));

            // Add some extra characters
            s.AddRange(RandomString(rng, 0, s.Count / 4));

            // Shuffle it and return it
            return s.Shuffle().ToList();
        }

        private static void ReplaceChar(Random rng, List<char> s)
        {
            var idx = rng.Next(0, s.Count);
            var original = s[idx];
            char chr;
            do
            {
                chr = RandomString(rng, 1, 1)[0];
            } while (chr == original);

            s[idx] = chr;
        }

        private static bool ArePermutation(string str1, string str2)
        {
            if (str1.Length != str2.Length)
                return false;

            var s1 = str1.OrderBy(a => a).ToList();
            var s2 = str2.OrderBy(a => a).ToList();

            for (var i = 0; i < str1.Length;  i++)
                if (s1[i] != s2[i])
                    return false;
 
            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class TheBells
    {
        private static string RandomString(Random rng, int minLength, int maxLength)
        {
            const string chars = "ABCDEFGH";

            var length = rng.Next(minLength, maxLength);
            var order = Enumerable.Repeat(chars, length)
                                  .Select(s => chars[rng.Next(chars.Length)])
                                  .SelectMany(c => rng.NextDouble() > 0.93f ? new string(c, rng.Next(1, 10)) : c.ToString())
                                  .ToArray();

            return string.Join("", order);
        }

        [TestMethod]
        public void GenerateTheBells()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            void SingleCase(string pattern)
            {
                input.Add(new Dictionary<string, Value> {
                    { "i", pattern },
                });

                var result = ApplyRules(pattern);

                output.Add(new Dictionary<string, Value> {
                    { "o", result },
                });
            }

            SingleCase("ABAB");
            SingleCase("AAA");
            SingleCase("BACA");
            SingleCase("BACAD");

            var r = new Random();
            while (input.Count < 100000)
                SingleCase(RandomString(r, 1, 7));

            Console.WriteLine($"Most swaps: {_most}");

            Generator.YololLadderGenerator(input, output);
        }

        private static int _most;
        private static string ApplyRules(string pattern)
        {
            // - If an A comes directly before a B, swap them
            // - If an A comes directly after a C, swap them
            // - If an C comes directly after a D, swap them
            // - If an C comes directly after a B, swap them
            // - If an D comes directly after a A, swap them
            // - If there are an odd number of items, add the first character to the end of the string

            static string ApplyRule(string input, Func<string, string> func)
            {
                var count = 0;
                string p;
                do
                {
                    p = input;
                    input = func(input);
                    count++;
                } while (input != p);

                _most = Math.Max(_most, count);

                return input;
            }

            // - If an A comes directly before a B, swap them
            if (pattern.Contains("AB", StringComparison.OrdinalIgnoreCase))
                return ApplyRule(pattern, a => a.Replace("AB", "BA"));

            // - If an A comes directly after a C, swap them
            if (pattern.Contains("CA", StringComparison.OrdinalIgnoreCase))
                return ApplyRule(pattern, a => a.Replace("CA", "AC"));

            // - If an C comes directly after a D, swap them
            if (pattern.Contains("DC", StringComparison.OrdinalIgnoreCase))
                return ApplyRule(pattern, a => a.Replace("DC", "CD"));

            // - If an C comes directly after a B, swap them
            if (pattern.Contains("BC", StringComparison.OrdinalIgnoreCase))
                return ApplyRule(pattern, a => a.Replace("BC", "CB"));

            // - If an D comes directly after a A, swap them
            if (pattern.Contains("AD", StringComparison.OrdinalIgnoreCase))
                return ApplyRule(pattern, a => a.Replace("AD", "DA"));

            // - If there are an odd number of items, add the first character to the end of the string
            if (pattern.Length % 2 == 1)
                return pattern + pattern[0];

            return pattern;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class FindAndReplace
    {
        private static string RandomString(Random rng, int minLength, int maxLength)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";

            var length = rng.Next(minLength, maxLength);
            var order = Enumerable.Repeat(chars, length)
                                  .Select(s => chars[rng.Next(chars.Length)])
                                  .ToArray();

            return string.Join("", order);
        }

        [TestMethod]
        public void GenerateFindAndReplace()
        {
            var rng = new Random(678345);
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            void Emit(string original, string find, string replace, string result = null)
            {
                if (result == null)
                    result = original.Replace(find, replace);

                input.Add(new Dictionary<string, Value> {{"a", original}, {"b", find}, {"c", replace}});
                output.Add(new Dictionary<string, Value> {{"o", result}});
            }

            void SingleCase()
            {
                if (rng.NextDouble() > 0.1f)
                {
                    var find = RandomString(rng, 3, 10);

                    var original = RandomString(rng, 0, 7);
                    var count = rng.Next(0, 4);
                    for (var i = 0; i < count; i++)
                    {
                        original += find;
                        original += RandomString(rng, 0, 7);
                    }

                    var replace = RandomString(rng, 0, 5);
                    var o = original.Replace(find, replace);

                    Emit(original, find, replace, o);
                }
                else
                {
                    Emit(
                        RandomString(rng, 5, 20),
                        RandomString(rng, 3, 10),
                        RandomString(rng, 0, 5)
                    );
                }
            }

            Emit("CYLON", "C", "P");
            Emit("YOLOL", "YO", "");
            Emit("Referee", "e", "i");
            Emit("Numb3rs", "3", "e");
            Emit("CaseSensitive", "s", "z");

            while (output.Count < 4000)
                SingleCase();

            Generator.YololLadderGenerator(input, output, true, Generator.ScoreMode.BasicScoring);
        }

        private static string Output(string[] parts)
        {
            if (parts.Length == 1)
                return parts[0];

            return string.Join(", ", parts[0..^1]) + " and " + parts.Last();
        }

        private static string Pluralise(int amount, string unit)
        {
            return $"{amount} {unit}" + (amount > 1 ? "s" : "");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class SubstringSearch
    {
        private static string RandomString(Random rng, int maxLength = 300)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz    -1234567890";

            var length = rng.Next(10, maxLength);
            var order = Enumerable.Repeat(chars, length)
                                  .Select(s => chars[rng.Next(chars.Length)])
                                  .ToArray();

            return string.Join("", order);
        }

        [TestMethod]
        public void GenerateSubstringSearch()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            void SingleCase(string haystack, string needle)
            {
                input.Add(new Dictionary<string, Value> {
                    { "h", haystack },
                    { "n", needle },
                });

                var index = haystack.IndexOf(needle, StringComparison.Ordinal);

                output.Add(new Dictionary<string, Value> {
                    { "o", (Number)index }
                });
            }

            void GenSingleCase(Random rng)
            {
                var haystack = RandomString(rng);
                var needle = RandomString(rng, haystack.Length);
                SingleCase(haystack, needle);
            }

            void GenSingleSubCase(Random rng)
            {
                var haystack = RandomString(rng);

                var start = rng.Next(0, haystack.Length);
                var length = rng.Next(0, haystack.Length - start);
                var needle = haystack.Substring(start, length);

                SingleCase(haystack, needle);
            }

            SingleCase("hello cylon", "hello cylon");
            SingleCase("Find the needle", "the");
            SingleCase("in the haystack", "haystack");
            SingleCase("or return -1", "return -1");
            SingleCase("good luck", "");

            var rng = new Random(6784);
            for (var i = 0; i < 14000; i++)
                GenSingleCase(rng);
            for (var x = 0; x < 15000; x++)
                GenSingleSubCase(rng);
            for (var i = 0; i < 1000; i++)
                SingleCase(RandomString(rng), "");

            Generator.YololLadderGenerator(input, output, true);
        }
    }
}

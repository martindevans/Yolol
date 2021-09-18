using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class Stringdexing
    {
        private static string RandomString(Random rng, int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz";

            var order = Enumerable.Repeat(chars, length)
                                  .Select(s => chars[rng.Next(chars.Length)])
                                  .ToArray();

            return string.Join("", order);
        }

        [TestMethod]
        public void GenerateStringdexing()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            void SingleCase(string value, int index)
            {
                input.Add(new Dictionary<string, Value> { { "s", value }, { "i", (Number)index } });
                output.Add(new Dictionary<string, Value> { { "o", value[index].ToString() } });
            }

            void GenSingleCase(Random rng)
            {
                var length = rng.Next(5, 100);
                if (rng.NextDouble() < 0.25)
                    length = rng.Next(100, 1024);

                var value = RandomString(rng, length);
                var index = rng.Next(value.Length);
                SingleCase(value, index);
            }

            SingleCase("hello cylon", 0);
            SingleCase("get the character", 1);
            SingleCase("from the string", 2);
            SingleCase("with a zero based index", 3);
            SingleCase("good luck", 4);

            var rng = new Random(345897);
            for (var x = 0; x < 50000; x++)
                GenSingleCase(rng);

            Generator.YololLadderGenerator(input, output, true);
        }
    }
}

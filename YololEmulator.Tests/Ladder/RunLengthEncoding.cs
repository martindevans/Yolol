using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class RunLengthEncoding
    {
        private static string RandomString(Random rng, int minLength, int maxLength)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

            var length = rng.Next(minLength, maxLength);
            var order = Enumerable.Repeat(chars, length)
                                  .Select(s => chars[rng.Next(chars.Length)])
                                  .SelectMany(c => rng.NextDouble() > 0.93f ? new string(c, rng.Next(1, 10)) : c.ToString())
                                  .ToArray();

            return string.Join("", order);
        }

        [TestMethod]
        public void GenerateRLE()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            static string RLE(string input)
            {
                if (input == "")
                    return input;

                var result = "";

                var count = 1;
                var prev = input[0];
                for (var i = 1; i < input.Length; i++)
                {
                    var c = input[i];
                    if (c == prev)
                        count++;
                    else
                    {
                        result += (count > 1 ? count.ToString() : "") + "" + prev;
                        count = 1;
                        prev = c;
                    }
                }

                result += (count > 1 ? count.ToString() : "") + "" + prev;

                return result;
            }

            void SingleCase(string value)
            {
                input.Add(new Dictionary<string, Value> { { "a", value } });
                output.Add(new Dictionary<string, Value> { { "o", RLE(value) } });
            }

            SingleCase("CYLON");
            SingleCase("Referee");
            SingleCase("Aaaaaargh");
            SingleCase("itsjustletters");
            SingleCase("nonumbers");

            var rng = new Random(345897);
            for (var x = 0; x < 8000; x++)
                SingleCase(RandomString(rng, 1, 10));

            Generator.YololLadderGenerator(input, output, true, Generator.ScoreMode.BasicScoring);
        }
    }
}

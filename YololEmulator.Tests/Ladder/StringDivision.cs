using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class StringDivision
    {
        private static string RandomString(Random rng, int minLength, int maxLength)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz";

            var length = rng.Next(minLength, maxLength);
            var order = Enumerable.Repeat(chars, length)
                                  .Select(s => chars[rng.Next(chars.Length)])
                                  .SelectMany(c => rng.NextDouble() > 0.93f ? new string(c, rng.Next(1, 10)) : c.ToString())
                                  .ToArray();

            return string.Join("", order);
        }

        [TestMethod]
        public void GenerateStringDivision()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            static int Matches(string input, string divisor)
            {
                return Regex.Matches(input, divisor).Count;

                //var offset = 0;
                //var count = 0;
                //while (offset < input.Length)
                //{
                //    var idx = input.IndexOf(divisor, offset, StringComparison.OrdinalIgnoreCase);
                //    if (idx == -1)
                //        break;
                //    offset = idx + 1;
                //    count++;
                //}

                //return count;
            }

            void SingleCase(string top, string bot)
            {
                input.Add(new Dictionary<string, Value> { { "n", top }, { "d", bot } });
                output.Add(new Dictionary<string, Value> { { "o", (Value)Matches(top, bot) } });
            }

            void GenSingleCase(Random rng)
            {
                var top = RandomString(rng, 5, 175);

                if (rng.NextDouble() < 0.05)
                {
                    SingleCase(top, RandomString(rng, 1, 4));
                    return;
                }

                var bestScore = 0;
                var best = (string?)null;
                for (var i = 0; i < 16; i++)
                {
                    var bot = RandomString(rng, 1, 3);
                    var m = Matches(top, bot);
                    var s = bot.Length * m * m;

                    if (s > bestScore)
                    {
                        best = bot;
                        bestScore = s;
                    }
                }

                if (best != null)
                    SingleCase(top, best);
            }

            SingleCase("hellocylon", "l");
            SingleCase("lowercaseonly", "o");
            SingleCase("lettersonly", "tt");
            SingleCase("nooverlapssss", "ss");
            SingleCase("goodluck", "g");

            var rng = new Random(345897);
            for (var x = 0; x < 10000; x++)
                GenSingleCase(rng);

            Generator.YololLadderGenerator(input, output, true, Generator.ScoreMode.BasicScoring);
        }
    }
}

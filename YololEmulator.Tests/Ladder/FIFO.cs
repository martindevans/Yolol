using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class FIFO
    {
        private static string RandomString(Random rng, int minLength, int maxLength)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

            var length = rng.Next(minLength, maxLength);
            var order = Enumerable.Repeat(chars, length)
                                  .Select(s => chars[rng.Next(chars.Length)])
                                  .ToArray();

            return string.Join("", order);
        }

        [TestMethod]
        public void GenerateFIFO()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            Queue<string> queue = new();
            void SingleCase(string str, bool locked)
            {
                input.Add(new Dictionary<string, Value> {
                    {"i", str},
                    {"l", (Number)locked}
                });
                if (str.Length > 0)
                    queue.Enqueue(str);

                var outp = new Dictionary<string, Value>();
                if (!locked)
                    outp.Add("o", queue.Dequeue());
                output.Add(outp);
            }

            SingleCase("Input1", true);
            SingleCase("Input2", false);
            SingleCase("", false);
            SingleCase("good", true);
            SingleCase("luck", false);

            var rng = new Random(346);
            for (var i = 0; i < 10000; i++)
            {
                var locked = true;
                if (queue.Count > 0)
                    locked = rng.NextDouble() > 0.5f;
                if (queue.Count > 20)
                    locked = false;

                SingleCase(RandomString(rng, 3, 8), locked);
            }

            Generator.YololLadderGenerator(input, output, shuffle: false);
        }
    }
}

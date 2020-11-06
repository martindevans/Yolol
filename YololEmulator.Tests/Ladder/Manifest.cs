using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class Manifest
    {
        private static string RandomString(Random rng, int maxChars, int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            // Select the characters we're working with for this order
            var selection = chars.OrderBy(_ => rng.NextDouble()).Take(maxChars).ToArray();

            var order = Enumerable.Repeat(chars, length)
                                  .Select(s => selection[rng.Next(selection.Length)])
                                  .ToArray();

            return string.Join("", order);
        }

        private static string ToManifest(string str)
        {
            return string.Join(",", str
               .GroupBy(a => a)
               .OrderBy(a => a.Key)
               .Select(a => a.Key + "" + a.Count())
               .ToArray()
            );
        }

        [TestMethod]
        public void GenerateManifest()
        {
            var inventory = new List<char>();
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            var rng = new Random(45897435);

            for (var i = 0; i < 5000; i++)
            {
                // Add some new items to the inventory
                var add = RandomString(rng, rng.Next(2, 7), rng.Next(0, 20));
                inventory.AddRange(add);

                // Generate a request from the warehouse
                var req = RandomString(rng, rng.Next(2, 7), rng.Next(0, 20));
                input.Add(new Dictionary<string, Value> { { "i", string.Join("", ToManifest(add)) }, { "r", ToManifest(req) } });

                // Filter request by what can actually be provided
                var actual = new string(req.Where(c => inventory.Remove(c)).ToArray());

                output.Add(new Dictionary<string, Value> { { "o", ToManifest(actual) } });
            }

            Generator.YololLadderGenerator(input, output, false, Generator.ScoreMode.BasicScoring);
        }
    }
}

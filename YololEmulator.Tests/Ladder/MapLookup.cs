using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class MapLookup
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
        public void GenerateMapLookup()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            var map = new Dictionary<string, string>();

            void SingleCase(string key, string? value)
            {
                input.Add(new() {
                    {"k", key},
                    {"v", value ?? (Value)Number.Zero }
                });

                // Get existing value from map
                if (map.TryGetValue(key, out var prev))
                    output.Add(new() { { "o", prev } });
                else
                    output.Add(new() { { "o", "" } });

                // Overwrite what was already there
                if (value != null)
                    map[key] = value;
            }

            SingleCase("a", "Cylon");
            SingleCase("a", "Yolol");
            SingleCase("a", null);
            SingleCase("b", "Good");
            SingleCase("c", "Luck");

            var rng = new Random(5687);
            for (var i = 0; i < 10000; i++)
                SingleCase(RandomString(rng, 1, 1), RandomString(rng, 2, 9));

            Generator.YololLadderGenerator(input, output, shuffle: false);
        }
    }
}

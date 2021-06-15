using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class StringLengthCounting
    {
        private static string RandomString(Random rng, int minLength, int maxLength)
        {
            const string chars = " ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!?";

            var length = rng.Next(minLength, maxLength);
            var order = Enumerable.Repeat(chars, length)
                                  .Select(s => chars[rng.Next(chars.Length)])
                                  .ToArray();

            return string.Join("", order);
        }

        [TestMethod]
        public void GenerateStringLengthCounting()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            void SingleCase(string str)
            {
                input.Add(new Dictionary<string, Value> {
                    { "i", str }
                });

                output.Add(new Dictionary<string, Value> {
                    { "o", (Value)str.Length },
                });
            }
            
            SingleCase("How Long Is A Piece Of String?");
            SingleCase("Count the number of characters");
            SingleCase("Min length is 5 chars");
            SingleCase("Max length is 50 chars!");
            SingleCase("Good Luck");

            var rng = new Random(346);
            for (var i = 0; i < 10000; i++)
                SingleCase(RandomString(rng, 5, 50));

            Generator.YololLadderGenerator(input, output);
        }
    }
}

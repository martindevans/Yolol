using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;
using System.Linq;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class Mastermind
    {
        private static string RandomString(Random rng, int minLength, int maxLength)
        {
            const string chars = "0123456789abcdefghijklmnopqrstuvwxyz";

            var length = rng.Next(minLength, maxLength);
            var order = Enumerable.Repeat(chars, length)
                                  .Select(s => chars[rng.Next(chars.Length)])
                                  .ToArray();

            return string.Join("", order);
        }

        [TestMethod]
        public void GenerateMastermind()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            void SingleCase(string secret, string guess)
            {
                if (secret.Length != guess.Length)
                    throw new InvalidOperationException("must be same length");

                var q = "";
                var e = "";
                var w = "";

                for (var i = 0; i < guess.Length; i++)
                {
                    var c = guess[i];

                    if (secret[i] == c)
                        e += "!";
                    else if (secret.Contains(c))
                        q += "?";
                    else
                        w += "_";
                }

                input.Add(new Dictionary<string, Value> {
                    { "s", new Value(secret) },
                    { "g", new Value(guess) },
                });

                output.Add(new Dictionary<string, Value> {
                    { "o", new Value(e + q + w) },
                });
            }

            SingleCase("secret", "aguess");
            SingleCase("AllWrong", "HERPDERP");
            SingleCase("AllCorrect", "AllCorrect");
            SingleCase("Exclamations", "ComeFirst123");
            SingleCase("ThenThe", "QuMarks");

            var rng = new Random(34598);
            for (var i = 0; i < 10000; i++)
            {
                var length = rng.Next(3, 13);
                SingleCase(RandomString(rng, length, length), RandomString(rng, length, length));
            }

            Generator.YololLadderGenerator(input, output);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class RunLengthDecoding
    {
        [TestMethod]
        public void GenerateRLD()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            void SingleCase(string plaintext)
            {
                var encoded = RunLengthEncoding.RLE(plaintext);

                input.Add(new Dictionary<string, Value> { { "a", encoded } });
                output.Add(new Dictionary<string, Value> { { "o", plaintext } });
            }

            SingleCase("CYLON");
            SingleCase("Referee");
            SingleCase("Aaaaaargh");
            SingleCase("itsjustletters");
            SingleCase("nonumbers");

            var rng = new Random(758345);
            for (var x = 0; x < 8000; x++)
                SingleCase(RunLengthEncoding.RandomString(rng, 1, 20));

            Generator.YololLadderGenerator(input, output);
        }
    }
}

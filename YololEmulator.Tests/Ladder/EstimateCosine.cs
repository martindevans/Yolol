using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;
using MoreLinq;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class EstimateCosine
    {
        [TestMethod]
        public void GenerateEstimateCosine()
        {
            var rng = new Random(57239);
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            void SingleCase(Number i)
            {
                var cosine = i.Cos();

                input.Add(new Dictionary<string, Value> {
                    { "i", i },
                });

                output.Add(new Dictionary<string, Value> {
                    { "o", cosine },
                });
            }

            SingleCase((Number)0);
            SingleCase((Number)90);
            SingleCase((Number)180);
            SingleCase((Number)270);
            SingleCase((Number)9999);

            for (var x = 0; x < 10000; x++)
            {
                var v = ((Number)rng.Next(1, 9999999)) / (Number)1000;
                SingleCase(v);
            }

            Generator.YololLadderGenerator(input, output, true, Generator.ScoreMode.Approximate, Generator.YololChip.Advanced);
        }
    }
}

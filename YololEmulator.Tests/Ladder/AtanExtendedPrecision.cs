using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class AtanExtendedPrecision
    {
        [TestMethod]
        public void GenerateAtanExtendedPrecision()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            void SingleCase(double value)
            {
                var yololInput = (Number)(value * 1000);
                value = ((double)yololInput) / 1000;

                var result = Math.Atan(value) * 360 / (Math.PI * 2);

                var yololOutput = (Number)(result * 1000);

                input.Add(new Dictionary<string, Value> {
                    { "i", yololInput }
                });
                output.Add(new Dictionary<string, Value> {
                    { "o", yololOutput }
                });
            }

            var rng = new Random(5873);

            SingleCase(10);
            SingleCase(20);
            SingleCase(-10);
            SingleCase(40);

            void GenRange(double range)
            {
                for (var i = 0; i < 1000; i++)
                {
                    var neg = rng.NextDouble() < 0.5 ? -1 : 1;
                    SingleCase(rng.NextDouble() * range * neg);
                }
            }

            for (var i = 1; i < 10; i++)
                GenRange(i);
            GenRange(50);

            Generator.YololLadderGenerator(input, output, true, Generator.ScoreMode.Approximate);
        }
    }
}
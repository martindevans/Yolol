using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class NthRootApprox
    {
        [TestMethod]
        public void MethodName()
        {
            var yololInput = (Number)66.669;
            var val = (double)yololInput;

            var root = Math.Pow(val, 1.0 / 2);
            Console.WriteLine(root);

            var round = Math.Round(root, 3);
            Console.WriteLine(round);

            var yololOutput = (Number)round;
            Console.WriteLine(yololOutput);

            Assert.AreEqual(8.164, (double)yololOutput);
        }

        [TestMethod]
        public void GenerateNthRootApprox()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            void SingleCase(double value, uint n)
            {
                var yololInput = (Number)value;
                var val = (double)yololInput;

                var root = Math.Pow(val, 1.0 / n);
                var yololOutput = (Number)Math.Round(root, 3);

                if (root > (double)Number.MaxValue || root < (double)Number.MinValue)
                    return;

                input.Add(new Dictionary<string, Value> {
                    { "i", yololInput },
                    { "n", (Value)n }
                });
                output.Add(new Dictionary<string, Value> {
                    { "o", yololOutput }
                });
            }

            var rng = new Random(5873);

            SingleCase(16, 1);
            SingleCase(16, 2);
            SingleCase(16, 4);
            SingleCase(1048576, 20);
            SingleCase(11, 3);

            // Generate cases with nice roots
            for (var i = 0; i < 10000; i++)
            {
                var root = rng.Next(0, 100);
                var pow = (uint)rng.Next(1, 5);
                SingleCase(Math.Pow(root, pow), pow);
            }

            // Generate cases with fairly small numbers
            for (var i = 0; i < 10000; i++)
            {
                var a = rng.Next(0, 100) + rng.NextDouble();
                var n = (uint)rng.Next(1, 5);
                SingleCase(a, n);
            }

            // Generate any old random cases
            for (var i = 0; i < 20000; i++)
            {
                SingleCase(rng.Next(1, 10000000) + rng.NextDouble(), (uint)rng.Next(1, 21));
            }

            Generator.YololLadderGenerator(input, output, true, Generator.ScoreMode.Approximate);
        }
    }
}
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class MakeItOne
        : BaseGenerator
    {
        [TestMethod]
        public void Generate()
        {
            Run(6782111, 250000, true, Generator.ScoreMode.Approximate);
        }

        protected override bool GenerateCase(Random random, int index, Dictionary<string, Value> inputs, Dictionary<string, Value> outputs)
        {
            // o = ax + (by)^2 + (cz)^3

            var y = (Number)((random.NextDouble() * 2 - 1) * 10);
            var b = (Number)random.Next(-99, 100);
            var by = b * y;
            var by2 = by * by;

            var z = (Number)((random.NextDouble() * 2 - 1) * 1);
            var c = (Number)random.Next(-99, 100);
            var cz = c * z;
            var cz3 = cz * cz * cz;

            var sum = by2 + cz3 - (Number)1;

            var a = (Number)random.Next(-99, 100);
            var x = -(double)sum / (double)a;
            var xn = (Number)x;

            var final = a * xn + (b * y).Exponent((Number)2) + (c * z).Exponent((Number)3);

            if (final != Number.One)
                return false;

            inputs.Add("x", xn);
            inputs.Add("y", y);
            inputs.Add("z", z);

            outputs.Add("o", Number.One);

            return true;
        }

        /// <summary>
        /// Find a pair of integers which multiply up to value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static (int, int)? TryFindPair(int value)
        {
            value = Math.Abs(value);

            for (var i = 98; i > 0; i--)
            {
                for (var j = 1; j < 99; j++)
                {
                    if (i * j == value)
                        return (i, j);
                }
            }

            return null;
        }
    }
}

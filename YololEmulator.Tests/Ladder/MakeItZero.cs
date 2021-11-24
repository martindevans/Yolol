using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class MakeItZero
        : BaseGenerator
    {
        [TestMethod]
        public void Generate()
        {
            Run(64545, 10000, true, Generator.ScoreMode.Approximate);
        }

        protected override bool GenerateCase(Random random, int index, Dictionary<string, Value> inputs, Dictionary<string, Value> outputs)
        {
            var ax = random.Next(-99, 100);
            var ay = random.Next(-99, 100);
            var a = ax * ay;

            var bx = random.Next(-99, 100);
            var by = random.Next(-99, 100);
            var b = bx * by;

            var cx = random.Next(-99, 100);
            var cy = random.Next(-99, 100);
            var c = cx * cy;

            var abc = a + b + c;

            var dd = TryFindPair(abc);
            if (dd is null)
                return false;

            var (dx, dy) = dd.Value;
            var d = dx * dy;

            if (a + b + c + d != 0)
            {
                dx = -dx;
                d = dx * dy;
            }

            if (a + b + c + d != 0)
                return false;

            inputs.Add("w", (Number)ax);
            inputs.Add("x", (Number)bx);
            inputs.Add("y", (Number)cx);
            inputs.Add("z", (Number)dx);

            outputs.Add("o", Number.Zero);

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

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoreLinq;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class BalanceTheScales
        : BaseGenerator
    {
        [TestMethod]
        public void Generate()
        {
            Run(679465, 25000, true, Generator.ScoreMode.BasicScoring);
        }

        protected override bool GenerateCase(Random random, int index, Dictionary<string, Value> inputs, Dictionary<string, Value> outputs)
        {
            var (target, w) = GenerateWeights(random);
            var weights = w.Shuffle().ToArray();

            // outputs the question
            inputs.Add("w", (Number)target);
            inputs.Add("a", (Number)weights[0]);
            inputs.Add("b", (Number)weights[1]);
            inputs.Add("c", (Number)weights[2]);
            inputs.Add("d", (Number)weights[3]);
            inputs.Add("e", (Number)weights[4]);
            inputs.Add("f", (Number)weights[5]);

            // And the answer
            outputs.Add("o", (Value)target);

            return true;
        }

        private static (int, int[]) GenerateWeights(Random random)
        {
            // Left side
            var a = random.Next(1, 25);
            var b = random.Next(1, 25);
            var c = random.Next(1, 25);
            var leftSum = a + b + c;

            // Right side
            var d = random.Next(1, 25);
            var e = random.Next(1, 25);
            var f = random.Next(1, 25);
            var rightSum = d + e + f;

            return (leftSum - rightSum, new [] { a, b, c, d, e, f });
        }
    }
}

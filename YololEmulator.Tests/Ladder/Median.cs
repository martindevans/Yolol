using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoreLinq;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class Median
        : BaseGenerator
    {
        [TestMethod]
        public void Generate()
        {
            Run(58567, 25000, true, Generator.ScoreMode.BasicScoring);
        }

        protected override bool GenerateCase(Random random, int index, Dictionary<string, Value> inputs, Dictionary<string, Value> outputs)
        {
            var values = new[]
            {
                random.Next(10), random.Next(10),
                random.Next(10), random.Next(10),
                random.Next(10), random.Next(10),
            };

            var median = values.OrderBy(a => a).ElementAt(values.Length / 2);

            // outputs the question
            inputs.Add("a", (Number)values[0]);
            inputs.Add("b", (Number)values[1]);
            inputs.Add("c", (Number)values[2]);
            inputs.Add("d", (Number)values[3]);
            inputs.Add("e", (Number)values[4]);
            inputs.Add("f", (Number)values[5]);

            // And the answer
            outputs.Add("o", (Value)median);

            return true;
        }
    }
}

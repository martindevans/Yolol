using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class ModeAverage
        : BaseGenerator
    {
        [TestMethod]
        public void Generate()
        {
            Run(58567, 25000, true, Generator.ScoreMode.BasicScoring);
        }

        protected override bool GenerateCase(Random random, int index, Dictionary<string, Value> inputs, Dictionary<string, Value> outputs)
        {
            var values = new List<int>
            {
                random.Next(10), random.Next(10),
                random.Next(10), random.Next(10)
            };
            values.Add(values[0]);
            values.Add(values[0]);
            values = values.Shuffle().ToList();

            var mode = values.GroupBy(n => n).OrderByDescending(a => a.Count()).ToList();

            // Skip indeterminate cases
            if (mode.Count > 1 && mode[0].Count() == mode[1].Count())
                return false;

            // outputs the question
            inputs.Add("a", (Number)values[0]);
            inputs.Add("b", (Number)values[1]);
            inputs.Add("c", (Number)values[2]);
            inputs.Add("d", (Number)values[3]);
            inputs.Add("e", (Number)values[4]);
            inputs.Add("f", (Number)values[5]);

            // And the answer
            outputs.Add("o", (Value)mode[0].Key);

            return true;
        }
    }
}

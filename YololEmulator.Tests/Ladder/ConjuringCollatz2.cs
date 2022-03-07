using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class ConjuringCollatz2
        : BaseGenerator
    {
        private int _maxLength = -1;

        [TestMethod]
        public void Generate()
        {
            Run(645745, 20000, true, Generator.ScoreMode.BasicScoring);
        }

        protected override bool GenerateCase(Random random, int index, Dictionary<string, Value> inputs, Dictionary<string, Value> outputs)
        {
            var length = ConjuringCollatz.CollatzLength(index + 1);
            _maxLength = Math.Max(_maxLength, length);

            // Skip some short chains
            if (length < 50 && random.NextDouble() < 0.5)
                return false;

            inputs.Add("i", (Number)(index + 1));
            outputs.Add("o", (Number)length);
            return true;
        }
    }
}

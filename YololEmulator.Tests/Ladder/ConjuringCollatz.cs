using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class ConjuringCollatz
        : BaseGenerator
    {
        [TestMethod]
        public void Generate()
        {
            Run(234587, 4000, true, Generator.ScoreMode.BasicScoring);
        }

        protected override bool GenerateCase(Random random, int index, Dictionary<string, Value> inputs, Dictionary<string, Value> outputs)
        {
            var l = CollatzLength(index + 1);
            if (l < 50)
            {
                inputs.Add("i", index + 1);
                outputs.Add("o", CollatzLength(index + 1));
                return true;
            }

            return false;
        }

        private int CollatzLength(int value)
        {
            // n → n/2 (n is even)
            // n → 3n + 1 (n is odd)

            var count = 1;
            while (value != 1)
            {
                count++;

                if (value % 2 == 0)
                    value /= 2;
                else
                    value = 3 * value + 1;
            }

            return count;
        }
    }
}

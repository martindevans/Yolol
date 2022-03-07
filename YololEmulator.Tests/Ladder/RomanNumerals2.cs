using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class RomanNumerals2
        : BaseGenerator
    {
        [TestMethod]
        public void Generate()
        {
            Run(68567, 3998, true, Generator.ScoreMode.BasicScoring);
        }

        protected override bool GenerateCase(Random random, int index, Dictionary<string, Value> inputs, Dictionary<string, Value> outputs)
        {
            index += 1;
            inputs.Add("i", RomanNumerals.ToRoman(index));
            outputs.Add("o", (Number)index);

            return true;
        }
    }
}

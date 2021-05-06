using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class BinaryAddition
        : BaseGenerator
    {
        [TestMethod]
        public void GenerateBinaryAddition()
        {
            Run(568456, 3000, true, Generator.ScoreMode.BasicScoring);
        }

        string B(uint value)
        {
            return Convert.ToString(value, 2).PadLeft(32, '0');
        }

        protected override bool GenerateCase(Random random, int index, Dictionary<string, Value> inputs, Dictionary<string, Value> outputs)
        {
            var a = (uint)random.Next();
            var b = (uint)random.Next();

            var bina = B(a);
            var binb = B(b);

            inputs.Add("a", bina);
            inputs.Add("b", binb);

            checked
            {
                var sum = B(a + b);
                outputs.Add("o", sum);
            }

            return true;
        }
    }
}

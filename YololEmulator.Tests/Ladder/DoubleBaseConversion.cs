using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class DoubleBaseConversion
    {
        [TestMethod]
        public void MultipleBaseConversion()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            var rng = new Random(5873);

            for (var i = 0; i < 2000; i++)
            {
                var inputNum = rng.Next(0, 5000);
                input.Add(new Dictionary<string, Value> {{"i", inputNum}});

                var octal = Convert.ToString(inputNum, 8);
                var binary = Convert.ToString(inputNum, 2);
                
                output.Add(new Dictionary<string, Value> {
                    { "o", octal },
                    { "b", binary },
                });
            }

            Generator.YololLadderGenerator(input, output, true, Generator.ScoreMode.BasicScoring);
        }
    }
}

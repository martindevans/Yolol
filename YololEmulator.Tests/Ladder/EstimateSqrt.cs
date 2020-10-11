using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class EstimateSqrt
    {
        [TestMethod]
        public void GenerateEstimateSqrt()
        {
            var rng = new Random(567908);
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            for (var x = 0; x < 2000; x++)
            {
                var a = (Number)(rng.NextDouble() * 10000);
                var c = (Number)Math.Sqrt((double)a);

                input.Add(new Dictionary<string, Value> { { "a", a } });
                output.Add(new Dictionary<string, Value> { { "o", c } });
            }

            Generator.YololLadderGenerator(input, output, true, Generator.ScoreMode.Approximate);
        }
    }
}

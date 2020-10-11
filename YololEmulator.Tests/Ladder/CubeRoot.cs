using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class CubeRoot
    {
        [TestMethod]
        public void GenerateCubeRoot()
        {
            var rng = new Random(4567456);
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            for (var x = 0; x < 5000; x++)
            {
                var a = (Number)(rng.NextDouble() * 10000);
                var c = (Number)Math.Pow((double)a, 1.0 / 3);

                input.Add(new Dictionary<string, Value> { { "a", a } });
                output.Add(new Dictionary<string, Value> { { "o", c } });
            }

            Generator.YololLadderGenerator(input, output, true, Generator.ScoreMode.Approximate, Generator.YololChip.Basic);
        }
    }
}
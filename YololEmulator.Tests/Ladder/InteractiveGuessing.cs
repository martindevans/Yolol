using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class InteractiveGuessing
    {
        [TestMethod]
        public void GenerateInteractiveGuessing()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            var rng = new Random(6573);

            for (var challenge = 0; challenge < 1000; challenge++)
            {
                var num = rng.Next(0, 1001);
                for (var i = 10; i >= 0; i--)
                {
                    input.Add(new Dictionary<string, Value> {{"countdown", i}});
                    output.Add(new Dictionary<string, Value> {{"o", num}});
                }
            }

            Generator.YololLadderGenerator(input, output, false, Generator.ScoreMode.Approximate);
        }
    }
}
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;
using System.Linq;
using MoreLinq;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class OneShotPid
    {
        [TestMethod]
        public void GenerateOneShotPid()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            var pp = (Number)0.752;
            var ii = (Number)0.010;
            var dd = (Number)0.565;

            void SingleCase(Number setpoint, Number process, Number integral, Number prevErr)
            {
                input.Add(new Dictionary<string, Value> {
                    { "s", setpoint },
                    { "p", process },
                    { "i", integral },
                    { "e", prevErr }
                });

                var err = setpoint - process;
                integral += err;
                var delta = err - prevErr;

                var pid = pp * err + ii * integral + dd * delta;

                output.Add(new Dictionary<string, Value> {
                    { "o", pid },
                    { "i", integral },
                    { "e", err },
                });
            }

            var rng = new Random(34897);
            for (var i = 0; i < 10000; i++)
            {
                SingleCase(
                    (Number)(rng.Next(-100, 100) / 100f),
                    (Number)(rng.Next(-300, 300) / 100f),
                    (Number)(rng.Next(-10, 10) / 100f),
                    (Number)(rng.Next(-50, 50) / 100f)
                );
            }

            Generator.YololLadderGenerator(input, output, mode: Generator.ScoreMode.Approximate);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class InverseKinematics
    {
        private const double JointLength0 = 2;
        private const double JointLength1 = 3;

        static float ToRadians(Number degrees)
        {
            var d = (double)degrees;
            d *= Math.PI * 2;
            d /= 360;
            return (float)d;
        }

        [TestMethod]
        public void GenerateInverseKinematics()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            void SingleCase(Number angle0, Number angle1)
            {
                var a0 = ToRadians(angle0);
                var a1 = ToRadians(angle1);

                var v0 = new Vector2((float)Math.Cos(a0), (float)Math.Sin(a0)) * (float)JointLength0;
                var v1 = new Vector2((float)Math.Cos(a0 + a1), (float)Math.Sin(a0 + a1)) * (float)JointLength1;
                var pos = v0 + v1;

                input.Add(new Dictionary<string, Value> {
                    { "x", (Number)Math.Round(pos.X, 3) },
                    { "y", (Number)Math.Round(pos.Y, 3) }
                });

                output.Add(new Dictionary<string, Value> {
                    { "a0", angle0 },
                    { "a1", angle1 }
                });
            }

            SingleCase((Number)0, (Number)0);
            SingleCase((Number)0, (Number)90);
            SingleCase((Number)90, (Number)0);
            SingleCase((Number)90, (Number)90);
            SingleCase((Number)45, (Number)0);

            var rng = new Random(234897);
            for (var i = 0; i < 100; i++)
            {
                for (var j = 0; j < 100; j++)
                {
                    var a0 = rng.NextDouble() + i / 100f * 89;
                    var a1 = rng.NextDouble() + j / 100f * 89;
                    SingleCase((Number)Math.Round(a0, 3), (Number)Math.Round(a1, 3));
                }
            }

            Generator.YololLadderGenerator(input, output, true, Generator.ScoreMode.Approximate);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class DriftingSpaceship
    {
        private float RandNormal(Random rand, float mean, float stdDev)
        {
            var u1 = 1.0 - rand.NextDouble();
            var u2 = 1.0 - rand.NextDouble();
            var randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            var randNormal =  mean + stdDev * randStdNormal;

            return (float)randNormal;
        }

        [TestMethod]
        public void GenerateDriftingSpaceship()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            var rng = new Random(345834);

            var dir = Vector3.UnitZ;
            var pos = Vector3.Zero;
            var cnt = 0;

            for (var i = 0; i < 2000; i++)
            {
                if (--cnt <= 0)
                {
                    dir = Vector3.Transform(Vector3.UnitX, Quaternion.CreateFromYawPitchRoll(
                        (float)(rng.NextDouble() * Math.PI * 2),
                        (float)(rng.NextDouble() * Math.PI * 2),
                        (float)(rng.NextDouble() * Math.PI * 2))
                    ) * ((float)rng.NextDouble() + 1) * 10;
                    cnt = rng.Next(25, 100);
                }

                pos += dir;

                input.Add(new Dictionary<string, Value> {
                    { "dx", (Number)(dir.X + RandNormal(rng, 0, 1)) },
                    { "dy", (Number)(dir.Y + RandNormal(rng, 0, 1)) },
                    { "dz", (Number)(dir.Z + RandNormal(rng, 0, 1)) },
                });
                output.Add(new Dictionary<string, Value> {
                    { "px", (Number)pos.X },
                    { "py", (Number)pos.Y },
                    { "pz", (Number)pos.Z }
                });
            }

            Generator.YololLadderGenerator(input, output, false, Generator.ScoreMode.Approximate);
        }
    }
}

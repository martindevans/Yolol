using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class SimpleNavigationPositioning
    {
        [TestMethod]
        public void GenerateSimpleNavigationPositioning()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            var beacons = new[] {
                (28315.983,	14274.694, 92145.269),
                (19522.454,	53912.719, 24633.070),
                (05817.744,	97945.275, 82659.224),
                (82188.145,	28074.805, 33777.522),
            };

            Number Distance((double x, double y, double z) beacon, Number x, Number y, Number z)
            {
                var dx = beacon.x - (double)x;
                var dy = beacon.y - (double)y;
                var dz = beacon.z - (double)z;

                var d = Math.Sqrt(dx * dx + dy * dy + dz * dz);

                return (Number)d;
            }

            void SingleCase(Number x, Number y, Number z)
            {
                output.Add(new Dictionary<string, Value> {
                    { "x", x },
                    { "y", y },
                    { "z", z },
                });

                input.Add(new Dictionary<string, Value> {
                    { "d0", Distance(beacons[0], x, y, z) },
                    { "d1", Distance(beacons[1], x, y, z) },
                    { "d2", Distance(beacons[2], x, y, z) },
                    { "d3", Distance(beacons[3], x, y, z) },
                });
            }

            var rng = new Random(5873);

            for (var i = 1; i < 25000; i++)
            {
                SingleCase(
                    ((Number)(rng.NextDouble() * 50000 + 25000)),
                    ((Number)(rng.NextDouble() * 50000 + 25000)),
                    ((Number)(rng.NextDouble() * 50000 + 25000))
                );
            }

            Generator.YololLadderGenerator(input, output, true, Generator.ScoreMode.Approximate);
        }
    }
}
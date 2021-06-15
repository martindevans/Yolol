using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;
using MoreLinq;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class UnitConversion
    {
        [TestMethod]
        public void GenerateUnitConversion()
        {
            var rng = new Random(57239);
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            var units = new[] {
                "m",
                "in",
                "nm",
                "km",
                "ft"
            };

            void SingleCase(decimal value, string inputUnit, string outputUnit)
            {
                var meters = inputUnit switch {
                    "m" => value,
                    "in" => value * 0.0254m,
                    "nm" => value * 1852,
                    "km" => value * 1000,
                    "ft" => value * 0.3048m,
                    _ => throw new InvalidOperationException("Unknown input unit")
                };

                var result = outputUnit switch {
                    "m" => meters,
                    "in" => meters / 0.0254m,
                    "nm" => meters / 1852,
                    "km" => meters / 1000,
                    "ft" => meters / 0.3048m,
                    _ => throw new InvalidOperationException("Unknown input unit")
                };

                var nv = (Number)value;
                Assert.AreEqual(nv.ToString(), value.ToString(CultureInfo.InvariantCulture));
                input.Add(new Dictionary<string, Value> {
                    { "v", nv },
                    { "a", inputUnit },
                    { "b", outputUnit }
                });

                output.Add(new Dictionary<string, Value> {
                    { "o", (Value)(Math.Round(result * 1000) / 1000) }
                });
            }

            SingleCase(1, "m", "km");
            SingleCase(1, "km", "m");
            SingleCase(12, "in", "ft");
            SingleCase(1, "nm", "km");
            SingleCase(17, "ft", "nm");

            for (var x = 0; x < 10000; x++)
            {
                var v = (decimal)rng.Next(1, 500000) / 1000;
                SingleCase(v, units.Shuffle(rng).First(), units.Shuffle(rng).First());
            }

            Generator.YololLadderGenerator(input, output, true, Generator.ScoreMode.Approximate);
        }
    }
}

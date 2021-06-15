using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class GreatCircleCarrots
    {
        [TestMethod]
        public void GenerateGreatCircle()
        {
            var rng = new Random(67235);
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            void SingleCase(double latitude0, double longitude0, double latitude1, double longitude1)
            {
                var gcd = GreatCircleDistance(latitude0, longitude0, latitude1, longitude1);
                if (!gcd.HasValue)
                    return;

                input.Add(new Dictionary<string, Value> {
                    { "lat_0", (Number)latitude0 },
                    { "lon_0", (Number)longitude0 },
                    { "lat_1", (Number)latitude1 },
                    { "lon_1", (Number)longitude1 },
                });

                var carrots = gcd.Value * (Number)3;

                output.Add(new Dictionary<string, Value> {
                    { "o", carrots },
                });
            }
            
            SingleCase(0, 0, 10, 0);
            SingleCase(0, 0, 45, 0);
            SingleCase(90, 0, 0, 0);
            SingleCase(0, 90, 0, 180);
            SingleCase(17, 7, 12, 11);

            for (int i = 0; i < 10000; i++)
            {
                SingleCase(
                    (double)(Number)rng.NextDouble() * 180,
                    (double)(Number)rng.NextDouble() * 180,
                    (double)(Number)rng.NextDouble() * 180,
                    (double)(Number)rng.NextDouble() * 180
                );

            }

            Generator.YololLadderGenerator(input, output, true, Generator.ScoreMode.Approximate);
        }

        static Number? GreatCircleDistance(double latitude0, double longitude0, double latitude1, double longitude1)
        {
            latitude0 = Radians(latitude0);
            longitude0 = Radians(longitude0);
            latitude1 = Radians(latitude1);
            longitude1 = Radians(longitude1);

            var deltaLat = latitude1 - latitude0;
            var deltaLon = longitude1 - longitude0;

            var t1 = Math.Pow(Math.Sin(deltaLat / 2), 2);
            var t2 = Math.Cos(latitude0) * Math.Cos(latitude1) * Math.Pow(Math.Sin(deltaLon / 2), 2);
            var a = t1 + t2;

            var c = 2 * Math.Asin(Math.Sqrt(a));

            const double radius = 6371;
            var d = radius * c;

            var rounded = Math.Ceiling(d);

            // Do not include any cases that are near the rounding threshold
            if (Math.Abs(d - rounded) < 0.01)
                return null;

            return (Number)rounded;
        }

        public static double Radians(double degrees)
        {
            var radians = (Math.PI / 180) * degrees;
            return (radians);
        }
    }
}
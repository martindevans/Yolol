using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;
using System.Linq;
using MoreLinq.Extensions;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class PrimeRibbons
    {
        [TestMethod]
        public void GeneratePrimeRibbons()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            var primes = PrimeNumberSieve
                .GeneratePrimes(100)
                .OrderBy(a => a)
                .Select(a => (Number)a)
                .ToList();

            var rng = new Random(5698245);
            for (var j = 0; j < 20000; j++)
            {
                var x = (Number)rng.Next(1, 50);
                var y = (Number)rng.Next(1, 50);
                var z = (Number)rng.Next(1, 50);

                var length = x + x + y + y + y + y + z + z + (Number)15;
                var prime = primes.First(a => a >= length);

                input.Add(new Dictionary<string, Value> { { "x", x }, { "y", y }, { "z", z } });
                output.Add(new Dictionary<string, Value> { { "o", prime } });
            }

            //xxyyyyzz

            Generator.YololLadderGenerator(input, output);
        }
    }
}
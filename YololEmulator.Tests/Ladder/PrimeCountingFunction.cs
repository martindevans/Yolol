using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class PrimeCountingFunction
        : BaseGenerator
    {
        private readonly Dictionary<int, bool> _cache = new();

        [TestMethod]
        public void Generate()
        {
            Run(34923, 10000, false, Generator.ScoreMode.BasicScoring);
        }

        protected override bool GenerateCase(Random random, int index, Dictionary<string, Value> inputs, Dictionary<string, Value> outputs)
        {
            var i = index + 1;
            inputs.Add("i", i);
            outputs.Add("o", PrimeCount(i));
            return true;
        }

        private int PrimeCount(int n)
        {
            var count = n >= 2 ? 1 : 0;
            for (var i = 3; i < n; i += 2)
                if (IsOddIntPrime(i))
                    count++;
            return count;
        }

        private bool IsOddIntPrime(int number)
        {
            if (number <= 1)
                return false;
            if (_cache.TryGetValue(number, out var cached))
                return cached;

            var boundary = (int)Math.Floor(Math.Sqrt(number));

            for (var i = 3; i <= boundary; i += 2)
            {
                if (number % i == 0)
                {
                    _cache[number] = false;
                    return false;
                }
            }

            _cache[number] = true;
            return true;        
        }
    }
}

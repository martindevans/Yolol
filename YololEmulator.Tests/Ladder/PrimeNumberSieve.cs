using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class PrimeNumberSieve
        : BaseGenerator
    {
        const int Count = 200000;

        private HashSet<int>? _primes;

        [TestMethod]
        public void Generate()
        {
            Run(678345, Count, false, Generator.ScoreMode.BasicScoring);
        }

        protected override bool GenerateCase(Random random, int index, Dictionary<string, Value> inputs, Dictionary<string, Value> outputs)
        {
            if (_primes == null)
                _primes = GeneratePrimes(Count);

            var i = index + 1;
            inputs.Add("i", (Value)i);
            outputs.Add("o", (Number)_primes.Contains(i));
            return true;
        }

        public static int ApproximateNthPrime(int nn)
        {
            var n = (double)nn;
            double p;
            if (nn >= 7022)
            {
                p = n * Math.Log(n) + n * (Math.Log(Math.Log(n)) - 0.9385);
            }
            else if (nn >= 6)
            {
                p = n * Math.Log(n) + n * Math.Log(Math.Log(n));
            }
            else if (nn > 0)
            {
                p = new int[] { 2, 3, 5, 7, 11 }[nn - 1];
            }
            else
            {
                p = 0;
            }
            return (int)p;
        }

        public static BitArray SieveOfEratosthenes(int limit)
        {
            BitArray bits = new(limit + 1, true) {
                [0] = false,
                [1] = false
            };
            for (var i = 0; i * i <= limit; i++)
            {
                if (bits[i])
                {
                    for (var j = i * i; j <= limit; j += i)
                        bits[j] = false;
                }
            }
            return bits;
        }

        public static HashSet<int> GeneratePrimes(int n)
        {
            var limit = ApproximateNthPrime(n);
            BitArray bits = SieveOfEratosthenes(limit);
            HashSet<int> primes = new();
            for (int i = 0, found = 0; i < limit && found < n; i++)
            {
                if (bits[i])
                {
                    primes.Add(i);
                    found++;
                }
            }

            return primes;
        }
    }
}
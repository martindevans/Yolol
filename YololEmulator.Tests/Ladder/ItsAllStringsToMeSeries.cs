using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class ItsAllStringsToMeSeries
    {
        //[TestMethod]
        //public void Mod()
        //{
        //    new AllStringsGenerator2((a, b) => a % b).Run(45698);
        //}

        //[TestMethod]
        //public void Add()
        //{
        //    new AllStringsGenerator2((a, b) => a + b).Run(76813);
        //}

        //[TestMethod]
        //public void Subtract()
        //{
        //    new AllStringsGenerator2((a, b) => a - b).Run(743443);
        //}

        //[TestMethod]
        //public void Multiply()
        //{
        //    new AllStringsGenerator2((a, b) => a * b).Run(6789546);
        //}

        //[TestMethod]
        //public void Divide()
        //{
        //    new AllStringsGenerator2((a, b) => a / b).Run(879);
        //}
    }

    public class AllStringsGenerator
        : BaseGenerator
    {
        private readonly Func<BigInteger, BigInteger> _func;

        public Generator.YololChip Chip { get; set; } = Generator.YololChip.Advanced;
        public Generator.ScoreMode Mode { get; set; } = Generator.ScoreMode.Approximate;

        public AllStringsGenerator(Func<BigInteger, BigInteger> func)
        {
            _func = func;
        }

        public void Run(int seed, int count = 50000)
        {
            Run(seed, count, true, Mode, Chip);
        }

        private static BigInteger Random(Random random)
        {
            const string? chars = "0123456789";
            var num = BigInteger.Parse(Enumerable.Range(0, random.Next(1, 25)).Select(a => chars[random.Next(chars.Length)]).ToArray());

            if (random.NextDouble() < 0.5f)
                return -num;
            else
                return num;
        }

        protected override bool GenerateCase(Random random, int index, Dictionary<string, Value> inputs, Dictionary<string, Value> outputs)
        {
            var a = Random(random);

            try
            {
                var o = _func(a);
                inputs.Add("a", a.ToString());
                outputs.Add("o", o.ToString());
            }
            catch
            {
                return false;
            }

            return true;
        }
    }

    public class AllStringsGenerator2
        : BaseGenerator
    {
        private readonly Func<BigInteger, BigInteger, BigInteger> _func;

        public Generator.YololChip Chip { get; set; } = Generator.YololChip.Professional;
        public Generator.ScoreMode Mode { get; set; } = Generator.ScoreMode.Approximate;

        public AllStringsGenerator2(Func<BigInteger, BigInteger, BigInteger> func)
        {
            _func = func;
        }

        public void Run(int seed, int count = 50000)
        {
            Run(seed, count, true, Mode, Chip);
        }

        private static BigInteger Random(Random random)
        {
            const string? chars = "0123456789";
            var num = BigInteger.Parse(Enumerable.Range(0, random.Next(1, 25)).Select(a => chars[random.Next(chars.Length)]).ToArray());

            if (random.NextDouble() < 0.5f)
                return -num;
            else
                return num;
        }

        protected override bool GenerateCase(Random random, int index, Dictionary<string, Value> inputs, Dictionary<string, Value> outputs)
        {
            var a = Random(random);
            var b = Random(random);

            try
            {
                var o = _func(a, b);
                inputs.Add("a", a.ToString());
                inputs.Add("b", b.ToString());
                outputs.Add("o", o.ToString());
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}

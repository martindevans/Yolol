using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class NotAsEasySeries
    {
        //[TestMethod]
        //public void Mod()
        //{
        //    new NotAsEasy2(-10000, 10000, 1, 1000, (a, b) => a % b) {
        //        Chip = Generator.YololChip.Basic,
        //    }.Run(798354);
        //}

        //[TestMethod]
        //public void Asin()
        //{
        //    new NotAsEasy(-1, 1, v => Math.Asin(v) * 360 / (Math.PI * 2)).Run(3478345);
        //}

        //[TestMethod]
        //public void Acos()
        //{
        //    new NotAsEasy(-1, 1, v => Math.Acos(v) * 360 / (Math.PI * 2)).Run(87453);
        //}

        //[TestMethod]
        //public void Atan()
        //{
        //    new NotAsEasy(-1, 1, v => Math.Atan(v) * 360 / (Math.PI * 2)).Run(65745);
        //}

        //[TestMethod]
        //public void Log()
        //{
        //    var a = (double)(Number)(double)22.935;
        //    Console.WriteLine(a);
        //    var b = Math.Log(a);
        //    Console.WriteLine(b);
        //    var c = Math.Round(b, 3);
        //    Console.WriteLine(c);
        //    var d = (Number)c;
        //    Console.WriteLine(d);

        //    new NotAsEasy(0.001, 25, Math.Log).Run(687243);
        //}

        //[TestMethod]
        //public void Sin()
        //{
        //    new NotAsEasy(0, 360, v => Math.Sin(v * (Math.PI * 2) / 360)).Run(515);
        //}

        //[TestMethod]
        //public void Cos()
        //{
        //    new NotAsEasy(0, 360, v => Math.Cos(v * (Math.PI * 2) / 360)).Run(87231);
        //}

        //[TestMethod]
        //public void Tan()
        //{
        //    new NotAsEasy(-89.999, 89.999, v => Math.Tan(v * (Math.PI * 2) / 360)).Run(71476456);
        //}
    }

    public class NotAsEasy
        : BaseGenerator
    {
        private readonly double _min;
        private readonly double _max;
        private readonly Func<double, double> _func;

        public NotAsEasy(double min, double max, Func<double, double> func)
        {
            _min = min;
            _max = max;
            _func = func;
        }

        public void Run(int seed, int count = 30000)
        {
            Run(seed, count, true, Generator.ScoreMode.Approximate, Generator.YololChip.Advanced);
        }

        protected override bool GenerateCase(Random random, int index, Dictionary<string, Value> inputs, Dictionary<string, Value> outputs)
        {
            var i = (Number)(random.NextDouble() * (_max - _min) + _min);
            if ((double)i < _min || (double)i > _max)
                return false;

            var o = _func((double)i);
            var oRound = (Number)Math.Round(o, 3);

            inputs.Add("i", i);
            outputs.Add("o", oRound);

            return true;
        }
    }

    public class NotAsEasy2
        : BaseGenerator
    {
        private readonly double _minA;
        private readonly double _maxA;
        private readonly double _minB;
        private readonly double _maxB;
        private readonly Func<double, double, double> _func;

        public Generator.YololChip Chip { get; set; } = Generator.YololChip.Advanced;
        public Generator.ScoreMode Mode { get; set; } = Generator.ScoreMode.Approximate;

        public NotAsEasy2(double mina, double maxa, double minb, double maxb, Func<double, double, double> func)
        {
            _minA = mina;
            _maxA = maxa;
            _minB = minb;
            _maxB = maxb;
            _func = func;
        }

        public void Run(int seed, int count = 50000)
        {
            Run(seed, count, true, Mode, Chip);
        }

        protected override bool GenerateCase(Random random, int index, Dictionary<string, Value> inputs, Dictionary<string, Value> outputs)
        {
            var a = (Number)(random.NextDouble() * (_maxA - _minA) + _minA);
            if ((double)a < _minA || (double)a > _maxA)
                return false;

            var b = (Number)(random.NextDouble() * (_maxB - _minB) + _minB);
            if ((double)b < _minB || (double)b > _maxB)
                return false;

            var o = _func((double)a, (double)b);
            var oRound = Math.Round(o, 3);
            var oNumber = (Number)oRound;

            inputs.Add("a", a);
            inputs.Add("b", b);
            outputs.Add("o", oNumber);

            return true;
        }
    }
}

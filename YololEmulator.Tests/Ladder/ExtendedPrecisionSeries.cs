using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class ExtendedPrecisionSeries
    {
        //[TestMethod]
        //public void Asin()
        //{
        //    // Challenge already done (using a slightly older form of the generator)
        //    new ExtendedPrecision(-1, 1, v => Math.Asin(v) * 360 / (Math.PI * 2)).Run(3478345);
        //}

        //[TestMethod]
        //public void Acos()
        //{
        //    new ExtendedPrecision(-1, 1, v => Math.Acos(v) * 360 / (Math.PI * 2)).Run(87453);
        //}

        //[TestMethod]
        //public void Log()
        //{
        //    new ExtendedPrecision(0.001, 55, Math.Log).Run(687243);
        //}

        //[TestMethod]
        //public void Sin()
        //{
        //    new ExtendedPrecision(0, 360, v => Math.Sin(v * (Math.PI * 2) / 360)).Run(515);
        //}

        //[TestMethod]
        //public void Tan()
        //{
        //    new ExtendedPrecision(-89.999, 89.999, v => Math.Tan(v * (Math.PI * 2) / 360)).Run(71476456);
        //}

        //[TestMethod]
        //public void Sqrt()
        //{
        //    // In Pool
        //    new ExtendedPrecision(0, 999_999, Math.Sqrt).Run(893784132);
        //}
    }

    public class ExtendedPrecision
        : BaseGenerator
    {
        private readonly double _min;
        private readonly double _max;
        private readonly Func<double, double> _func;

        public ExtendedPrecision(double min, double max, Func<double, double> func)
        {
            _min = min;
            _max = max;
            _func = func;
        }

        public void Run(int seed, int count = 300000)
        {
            Run(seed, count, true, Generator.ScoreMode.Approximate, Generator.YololChip.Professional);
        }

        protected override bool GenerateCase(Random random, int index, Dictionary<string, Value> inputs, Dictionary<string, Value> outputs)
        {
            var step = (_max - _min) / Count;
            var value = index switch {
                0 => _min,
                1 => _max,
                _ => step * index + (random.NextDouble() - 0.5) * step
            };

            var yololInput = (Number)(value * 1000);
            value = ((double)yololInput) / 1000;

            var result = _func(value);

            var yololOutput = (Number)(result * 1000);

            inputs.Add("i", yololInput);
            outputs.Add("o", yololOutput);

            return true;
        }
    }
}

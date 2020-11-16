﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Expressions.Num
{
    [TestClass]
    public class Exponent
    {
        [TestMethod]
        public void ExpNumConstantConstant()
        {
            var result = TestExecutor.Execute("a = 6 ^ 2");

            var a = result.GetVariable("a");

            Assert.AreEqual(36, (int)a.Value.Number);
        }

        [TestMethod]
        public void ExpNumVariableConstant()
        {
            var result = TestExecutor.Execute("a = 5", "b = a ^ 2");

            var a = result.GetVariable("a");
            var b = result.GetVariable("b");

            Assert.AreEqual(5, (int)a.Value.Number);
            Assert.AreEqual(25, (int)b.Value.Number);
        }

        [TestMethod]
        public void ExpNumConstantVariable()
        {
            var result = TestExecutor.Execute("a = 2", "b = 6 ^ a");

            var a = result.GetVariable("a");
            var b = result.GetVariable("b");

            Assert.AreEqual(2, (int)a.Value.Number);
            Assert.AreEqual(36, (int)b.Value.Number);
        }

        [TestMethod]
        public void Artturi()
        {
            // Results from the actual game, provided by Artturi
            // https://discordapp.com/channels/590281942184755239/593510152406171707/619082319487172608

            decimal Calc(string code) => (decimal)TestExecutor.Execute(code).GetVariable("a").Value.Number;

            Assert.AreEqual(0.008m, Calc("a = 5 ^ -3"));
            Assert.AreEqual(-0.008m, Calc("a = -5 ^ -3"));
            Assert.AreEqual(165.809m, Calc("a = 5.2 ^ 3.1"));
            Assert.AreEqual(0.006m, Calc("a = 5.2 ^ -3.1"));

            Assert.AreEqual((decimal)Number.MinValue, Calc("a = -1 ^ -0.5"));
        }

        [TestMethod]
        public void HugeExponent()
        {
            var a = TestExecutor.Execute("a = 1000000000000000000000000000000").GetVariable("a").Value;

            Assert.AreEqual(Number.MaxValue, a);
        }
    }
}

﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YololEmulator.Tests.Scripts.Basic.Num
{
    [TestClass]
    public class Division
    {
        [TestMethod]
        public void DivNumConstantConstant()
        {
            var result = TestExecutor.Execute("a = 6 / 2");

            var a = result.Get("a");

            Assert.AreEqual(3, a.Value.Number);
        }

        [TestMethod]
        public void DivNumVariableConstant()
        {
            var result = TestExecutor.Execute("a = 6", "b = a / 3");

            var a = result.Get("a");
            var b = result.Get("b");

            Assert.AreEqual(6, a.Value.Number);
            Assert.AreEqual(2, b.Value.Number);
        }

        [TestMethod]
        public void DivNumConstantVariable()
        {
            var result = TestExecutor.Execute("a = 2", "b = 6 / a");

            var a = result.Get("a");
            var b = result.Get("b");

            Assert.AreEqual(2, a.Value.Number);
            Assert.AreEqual(3, b.Value.Number);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YololEmulator.Tests.AST
{
    [TestClass]
    public class CompoundAssignment
    {
        [TestMethod]
        public void AppliesOperation()
        {
            var r = TestExecutor.Execute("a = 1 a *= 2");

            Assert.AreEqual(2, r.GetVariable("a").Value.Number);
        }

        [TestMethod]
        public void Equality()
        {
            var a = TestExecutor.Parse("a *= 1");
            var b = TestExecutor.Parse("a *= 1");

            Assert.IsTrue(a.Equals(b));
        }

        [TestMethod]
        public void Inequality()
        {
            var a = TestExecutor.Parse("a += 1");
            var b = TestExecutor.Parse("a *= 1");

            Assert.IsFalse(a.Equals(b));
        }
    }
}

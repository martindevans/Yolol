using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Expressions.Num
{
    [TestClass]
    public class Factorial
    {
        [TestMethod]
        public void Positive0()
        {
            var result = TestExecutor.Execute("a = 0!");

            var a = result.GetVariable("a");

            Assert.AreEqual(1, (int)a.Value.Number);
        }

        [TestMethod]
        public void Positive1()
        {
            var result = TestExecutor.Execute("a = 1!");

            var a = result.GetVariable("a");

            Assert.AreEqual(1, (int)a.Value.Number);
        }

        [TestMethod]
        public void Positive7()
        {
            var result = TestExecutor.Execute("a = 7!");

            var a = result.GetVariable("a");

            Assert.AreEqual(5040, (int)a.Value.Number);
        }

        [TestMethod]
        public void Negative()
        {
            var result = TestExecutor.Execute("a = (-4)!");

            var a = result.GetVariable("a");

            Assert.AreEqual(Number.MinValue, a.Value.Number);
        }

        [TestMethod]
        public void EqualityConfusion()
        {
            var result = TestExecutor.Execute("a = 7!==5039");

            var a = result.GetVariable("a");

            Assert.AreEqual(0, (int)a.Value.Number);
        }

        [TestMethod]
        public void EqualityConfusion2()
        {
            var result = TestExecutor.Execute("a = 7!!=5040");

            var a = result.GetVariable("a");

            Assert.AreEqual(0, (int)a.Value.Number);
        }
    }
}

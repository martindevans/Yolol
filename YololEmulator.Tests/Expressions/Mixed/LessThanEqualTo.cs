using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Expressions.Mixed
{
    [TestClass]
    public class LessThanEqualTo
    {
        [TestMethod]
        public void NumberStringFalse()
        {
            var result = TestExecutor.Execute("a = 3 <= \"2\"");

            var a = result.GetVariable("a");

            Assert.AreEqual(0, (int)a.Value.Number);
        }

        [TestMethod]
        public void StringNumberFalse()
        {
            var result = TestExecutor.Execute("a = \"3\" <= 2");

            var a = result.GetVariable("a");

            Assert.AreEqual(0, (int)a.Value.Number);
        }

        [TestMethod]
        public void NumberStringTrue()
        {
            var result = TestExecutor.Execute("a = 1 <= \"1\"");

            var a = result.GetVariable("a");

            Assert.AreEqual(1, (int)a.Value.Number);
        }

        [TestMethod]
        public void StringNumberTrue()
        {
            var result = TestExecutor.Execute("a = \"2\" <= 2");

            var a = result.GetVariable("a");

            Assert.AreEqual(1, (int)a.Value.Number);
        }

        [TestMethod]
        public void NumberVStringFalse()
        {
            var n = (Number)3;
            var v = new Value("2");

            Assert.IsFalse(n <= v);
        }

        [TestMethod]
        public void NumberVStringTrue()
        {
            var n = (Number)3;
            var v = new Value("3");

            Assert.IsTrue(n <= v);
        }
    }
}

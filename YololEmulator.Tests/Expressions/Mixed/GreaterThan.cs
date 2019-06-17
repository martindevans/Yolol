using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YololEmulator.Tests.Expressions.Mixed
{
    [TestClass]
    public class GreaterThan
    {
        [TestMethod]
        public void StringNumberFalse()
        {
            var result = TestExecutor.Execute("a = 1 > \"2\"");

            var a = result.Get("a");

            Assert.AreEqual(0, a.Value.Number);
        }

        [TestMethod]
        public void NumberStringFalse()
        {
            var result = TestExecutor.Execute("a = \"1\" > 2");

            var a = result.Get("a");

            Assert.AreEqual(0, a.Value.Number);
        }

        [TestMethod]
        public void NumberStringTrue()
        {
            var result = TestExecutor.Execute("a = 2 > \"1\"");

            var a = result.Get("a");

            Assert.AreEqual(1, a.Value.Number);
        }

        [TestMethod]
        public void StringNumberTrue()
        {
            var result = TestExecutor.Execute("a = \"2\" > 1");

            var a = result.Get("a");

            Assert.AreEqual(1, a.Value.Number);
        }
    }
}

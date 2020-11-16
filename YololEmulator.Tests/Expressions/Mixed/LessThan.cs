using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YololEmulator.Tests.Expressions.Mixed
{
    [TestClass]
    public class LessThan
    {
        [TestMethod]
        public void StringNumberFalse()
        {
            var result = TestExecutor.Execute("a = 3 < \"2\"");

            var a = result.GetVariable("a");

            Assert.AreEqual(0, (int)a.Value.Number);
        }

        [TestMethod]
        public void NumberStringFalse()
        {
            var result = TestExecutor.Execute("a = \"3\" < 2");

            var a = result.GetVariable("a");

            Assert.AreEqual(0, (int)a.Value.Number);
        }

        [TestMethod]
        public void StringNumberTrue()
        {
            var result = TestExecutor.Execute("a = \"a\" > 2");

            var a = result.GetVariable("a");

            Assert.AreEqual(1, (int)a.Value.Number);
        }

        [TestMethod]
        public void NumberStringTrue()
        {
            var result = TestExecutor.Execute("a = 2 > \"1\"");

            var a = result.GetVariable("a");

            Assert.AreEqual(1, (int)a.Value.Number);
        }
    }
}

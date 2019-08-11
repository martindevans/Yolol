using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YololEmulator.Tests.Expressions.Mixed
{
    [TestClass]
    public class NonEquality
    {
        [TestMethod]
        public void StringNumberTrueEqual()
        {
            var result = TestExecutor.Execute("a = 2 != \"2\"");

            var a = result.GetVariable("a");

            Assert.AreEqual(1, a.Value.Number);
        }

        [TestMethod]
        public void NumberStringTrueEqual()
        {
            var result = TestExecutor.Execute("a = \"2\" != 2");

            var a = result.GetVariable("a");

            Assert.AreEqual(1, a.Value.Number);
        }

        [TestMethod]
        public void StringNumberTrue()
        {
            var result = TestExecutor.Execute("a = 12 != \"1\"");

            var a = result.GetVariable("a");

            Assert.AreEqual(1, a.Value.Number);
        }

        [TestMethod]
        public void NumberStringTrue()
        {
            var result = TestExecutor.Execute("a = \"1\" != 11");

            var a = result.GetVariable("a");

            Assert.AreEqual(1, a.Value.Number);
        }
    }
}

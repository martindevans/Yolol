using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YololEmulator.Tests.Expressions.Num
{
    [TestClass]
    public class LessThan
    {
        [TestMethod]
        public void False()
        {
            var result = TestExecutor.Execute("a = 2 < 1");

            var a = result.Get("a");

            Assert.AreEqual(0, a.Value.Number);
        }

        [TestMethod]
        public void True()
        {
            var result = TestExecutor.Execute("a = 1 < 2");

            var a = result.Get("a");

            Assert.AreEqual(1, a.Value.Number);
        }
    }
}

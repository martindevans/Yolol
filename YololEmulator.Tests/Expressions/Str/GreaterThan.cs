using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YololEmulator.Tests.Expressions.Str
{
    [TestClass]
    public class GreaterThan
    {
        [TestMethod]
        public void False()
        {
            var result = TestExecutor.Execute("a = \"abc\" > \"xyz\"");

            var a = result.Get("a");

            Assert.AreEqual(0, a.Value.Number);
        }

        [TestMethod]
        public void True()
        {
            var result = TestExecutor.Execute("a = \"xyz\" > \"abc\"");

            var a = result.Get("a");

            Assert.AreEqual(1, a.Value.Number);
        }
    }
}

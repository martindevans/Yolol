using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Expressions.Num
{
    [TestClass]
    public class Sqrt
    {
        [TestMethod]
        public void Positive()
        {
            var result = TestExecutor.Execute("a = sqrt(1)");

            var a = result.GetVariable("a");

            Assert.AreEqual(1f, (float)a.Value.Number);
        }

        [TestMethod]
        public void Negative()
        {
            var result = TestExecutor.Execute("a = sqrt(-1)");

            var a = result.GetVariable("a");

            Assert.AreEqual(Number.MinValue, a.Value.Number);
        }
    }
}

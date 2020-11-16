using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YololEmulator.Tests.Expressions.Str
{
    [TestClass]
    public class GreaterThanEqualTo
    {
        [TestMethod]
        public void False()
        {
            var result = TestExecutor.Execute("a = \"abc\" >= \"xyz\"");

            var a = result.GetVariable("a");

            Assert.AreEqual(0, (int)a.Value.Number);
        }

        [TestMethod]
        public void True()
        {
            var result = TestExecutor.Execute("a = \"abc\" >= \"abc\"");

            var a = result.GetVariable("a");

            Assert.AreEqual(1, (int)a.Value.Number);
        }
    }
}

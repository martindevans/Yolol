using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YololEmulator.Tests.Expressions.Num
{
    [TestClass]
    public class Not
    {
        [TestMethod]
        public void NotConstantTrue()
        {
            var result = TestExecutor.Execute("a = not 1");

            var a = result.GetVariable("a");

            Assert.AreEqual(0, a.Value.Number);
        }

        [TestMethod]
        public void NotConstantFalse()
        {
            var result = TestExecutor.Execute("a = not 0");

            var a = result.GetVariable("a");

            Assert.AreEqual(1, a.Value.Number);
        }
    }
}
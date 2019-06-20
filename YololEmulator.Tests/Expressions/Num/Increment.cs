using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YololEmulator.Tests.Expressions.Num
{
    [TestClass]
    public class Increment
    {
        [TestMethod]
        public void PostIncrement()
        {
            var result = TestExecutor.Execute("a = 1", "b = a++");

            var a = result.GetVariable("a");
            var b = result.GetVariable("b");

            Assert.AreEqual(2, a.Value.Number);
            Assert.AreEqual(1, b.Value.Number);
        }

        [TestMethod]
        public void PreIncrement()
        {
            var result = TestExecutor.Execute("a = 1", "b = ++a");

            var a = result.GetVariable("a");
            var b = result.GetVariable("b");

            Assert.AreEqual(2, a.Value.Number);
            Assert.AreEqual(2, b.Value.Number);
        }
    }
}

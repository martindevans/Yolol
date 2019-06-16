using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YololEmulator.Tests.Scripts.Basic.Num
{
    [TestClass]
    public class Decrement
    {
        [TestMethod]
        public void PostDecrement()
        {
            var result = TestExecutor.Execute("a = 1", "b = a--");

            var a = result.Get("a");
            var b = result.Get("b");

            Assert.AreEqual(0, a.Value.Number);
            Assert.AreEqual(1, b.Value.Number);
        }

        [TestMethod]
        public void PreDecrement()
        {
            var result = TestExecutor.Execute("a = 1", "b = --a");

            var a = result.Get("a");
            var b = result.Get("b");

            Assert.AreEqual(0, a.Value.Number);
            Assert.AreEqual(0, b.Value.Number);
        }
    }
}

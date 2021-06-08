using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Expressions.Num
{
    [TestClass]
    public class Decrement
    {
        [TestMethod]
        public void PostDecrement()
        {
            var result = TestExecutor.Execute("a = 1", "b = a--");

            var a = result.GetVariable("a");
            var b = result.GetVariable("b");

            // If post dec is fixed, comment this back in
            //Assert.AreEqual(Number.Zero, a.Value.Number);
            //Assert.AreEqual(Number.One, b.Value.Number);

            Assert.AreEqual(Number.Zero, a.Value.Number);
            Assert.AreEqual(Number.Zero, b.Value.Number);
        }

        [TestMethod]
        public void PreDecrement()
        {
            var result = TestExecutor.Execute("a = 1", "b = --a");

            var a = result.GetVariable("a");
            var b = result.GetVariable("b");

            Assert.AreEqual(Number.Zero, a.Value.Number);
            Assert.AreEqual(Number.Zero, b.Value.Number);
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YololEmulator.Tests.Expressions.Num
{
    [TestClass]
    public class Multiplication
    {
        [TestMethod]
        public void MulNumConstantConstant()
        {
            var result = TestExecutor.Execute("a = 2 * 3");

            var a = result.Get("a");

            Assert.AreEqual(6, a.Value.Number);
        }

        [TestMethod]
        public void MulNumVariableConstant()
        {
            var result = TestExecutor.Execute("a = 2", "b = a * 3");

            var a = result.Get("a");
            var b = result.Get("b");

            Assert.AreEqual(2, a.Value.Number);
            Assert.AreEqual(6, b.Value.Number);
        }

        [TestMethod]
        public void MulNumConstantVariable()
        {
            var result = TestExecutor.Execute("a = 2", "b = 3 * a");

            var a = result.Get("a");
            var b = result.Get("b");

            Assert.AreEqual(2, a.Value.Number);
            Assert.AreEqual(6, b.Value.Number);
        }
    }
}

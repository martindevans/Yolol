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

            var a = result.GetVariable("a");

            Assert.AreEqual(6, (int)a.Value.Number);
        }

        [TestMethod]
        public void MulNumVariableConstant()
        {
            var result = TestExecutor.Execute("a = 2", "b = a * 3");

            var a = result.GetVariable("a");
            var b = result.GetVariable("b");

            Assert.AreEqual(2, (int)a.Value.Number);
            Assert.AreEqual(6, (int)b.Value.Number);
        }

        [TestMethod]
        public void MulNumConstantVariable()
        {
            var result = TestExecutor.Execute("a = 2", "b = 3 * a");

            var a = result.GetVariable("a");
            var b = result.GetVariable("b");

            Assert.AreEqual(2, (int)a.Value.Number);
            Assert.AreEqual(6, (int)b.Value.Number);
        }
    }
}

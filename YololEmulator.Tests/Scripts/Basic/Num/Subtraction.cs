using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YololEmulator.Tests.Scripts.Basic.Num
{
    [TestClass]
    public class Subtraction
    {
        [TestMethod]
        public void ConstantConstant()
        {
            var result = TestExecutor.Execute("a = 1 - 2");

            var a = result.Get("a");

            Assert.AreEqual(-1, a.Value.Number);
        }

        [TestMethod]
        public void VariableConstant()
        {
            var result = TestExecutor.Execute("a = 3", "b = a - 2");

            var a = result.Get("a");
            var b = result.Get("b");

            Assert.AreEqual(3, a.Value.Number);
            Assert.AreEqual(1, b.Value.Number);
        }

        [TestMethod]
        public void ConstantVariable()
        {
            var result = TestExecutor.Execute("a = 2", "b = 3 - a");

            var a = result.Get("a");
            var b = result.Get("b");

            Assert.AreEqual(2, a.Value.Number);
            Assert.AreEqual(1, b.Value.Number);
        }
    }
}

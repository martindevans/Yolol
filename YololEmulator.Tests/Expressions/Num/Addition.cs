using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YololEmulator.Tests.Expressions.Num
{
    [TestClass]
    public class Addition
    {
        [TestMethod]
        public void ConstantConstant()
        {
            var result = TestExecutor.Execute("a = 1 + 2");

            var a = result.GetVariable("a");

            Assert.AreEqual(3, a.Value.Number);
        }

        [TestMethod]
        public void VariableConstant()
        {
            var result = TestExecutor.Execute("a = 1", "b = a + 1");

            var a = result.GetVariable("a");
            var b = result.GetVariable("b");

            Assert.AreEqual(1, a.Value.Number);
            Assert.AreEqual(2, b.Value.Number);
        }

        [TestMethod]
        public void ConstantVariable()
        {
            var result = TestExecutor.Execute("a = 1", "b = 1 + a");

            var a = result.GetVariable("a");
            var b = result.GetVariable("b");

            Assert.AreEqual(1, a.Value.Number);
            Assert.AreEqual(2, b.Value.Number);
        }
    }
}

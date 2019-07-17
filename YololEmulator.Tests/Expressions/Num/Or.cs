using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YololEmulator.Tests.Expressions.Num
{
    [TestClass]
    public class Or
    {
        [TestMethod]
        public void ConstantConstantTrue()
        {
            var result = TestExecutor.Execute("a = 1 or 2");

            var a = result.GetVariable("a");

            Assert.AreEqual(1, a.Value.Number);
        }

        [TestMethod]
        public void ConstantConstantFalse()
        {
            var result = TestExecutor.Execute("a = 0 or 0");

            var a = result.GetVariable("a");

            Assert.AreEqual(0, a.Value.Number);
        }

        [TestMethod]
        public void VariableConstantTrue()
        {
            var result = TestExecutor.Execute("a = 1", "b = a or 0");

            var a = result.GetVariable("a");
            var b = result.GetVariable("b");

            Assert.AreEqual(1, a.Value.Number);
            Assert.AreEqual(1, b.Value.Number);
        }

        [TestMethod]
        public void VariableConstantFalse()
        {
            var result = TestExecutor.Execute("a = 0", "b = a or 0");

            var a = result.GetVariable("a");
            var b = result.GetVariable("b");

            Assert.AreEqual(0, a.Value.Number);
            Assert.AreEqual(0, b.Value.Number);
        }

        [TestMethod]
        public void ConstantVariableTrue()
        {
            var result = TestExecutor.Execute("a = 1", "b = 1 or a");

            var a = result.GetVariable("a");
            var b = result.GetVariable("b");

            Assert.AreEqual(1, a.Value.Number);
            Assert.AreEqual(1, b.Value.Number);
        }

        [TestMethod]
        public void ConstantVariableFalse()
        {
            var result = TestExecutor.Execute("a = 0", "b = 0 or a");

            var a = result.GetVariable("a");
            var b = result.GetVariable("b");

            Assert.AreEqual(0, a.Value.Number);
            Assert.AreEqual(0, b.Value.Number);
        }
    }
}

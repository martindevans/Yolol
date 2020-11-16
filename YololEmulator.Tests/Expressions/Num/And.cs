using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YololEmulator.Tests.Expressions.Num
{
    [TestClass]
    public class And
    {
        [TestMethod]
        public void ConstantConstantTrue()
        {
            var result = TestExecutor.Execute("a = 1 and 2");

            var a = result.GetVariable("a");

            Assert.AreEqual(1, (int)a.Value.Number);
        }

        [TestMethod]
        public void ConstantConstantFalse()
        {
            var result = TestExecutor.Execute("a = 0 and 0");

            var a = result.GetVariable("a");

            Assert.AreEqual(0, (int)a.Value.Number);
        }

        [TestMethod]
        public void VariableConstantTrue()
        {
            var result = TestExecutor.Execute("a = 1", "b = a and 1");

            var a = result.GetVariable("a");
            var b = result.GetVariable("b");

            Assert.AreEqual(1, (int)a.Value.Number);
            Assert.AreEqual(1, (int)b.Value.Number);
        }

        [TestMethod]
        public void VariableConstantFalse()
        {
            var result = TestExecutor.Execute("a = 0", "b = a and 10");

            var a = result.GetVariable("a");
            var b = result.GetVariable("b");

            Assert.AreEqual(0, (int)a.Value.Number);
            Assert.AreEqual(0, (int)b.Value.Number);
        }

        [TestMethod]
        public void ConstantVariableTrue()
        {
            var result = TestExecutor.Execute("a = 1", "b = 1 and a");

            var a = result.GetVariable("a");
            var b = result.GetVariable("b");

            Assert.AreEqual(1, (int)a.Value.Number);
            Assert.AreEqual(1, (int)b.Value.Number);
        }

        [TestMethod]
        public void ConstantVariableFalse()
        {
            var result = TestExecutor.Execute("a = 0", "b = 10 and a");

            var a = result.GetVariable("a");
            var b = result.GetVariable("b");

            Assert.AreEqual(0, (int)a.Value.Number);
            Assert.AreEqual(0, (int)b.Value.Number);
        }
    }
}

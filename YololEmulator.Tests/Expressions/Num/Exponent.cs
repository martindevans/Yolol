using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YololEmulator.Tests.Expressions.Num
{
    [TestClass]
    public class Exponent
    {
        [TestMethod]
        public void DivNumConstantConstant()
        {
            var result = TestExecutor.Execute("a = 6 ^ 2");

            var a = result.GetVariable("a");

            Assert.AreEqual(36, a.Value.Number);
        }

        [TestMethod]
        public void DivNumVariableConstant()
        {
            var result = TestExecutor.Execute("a = 5", "b = a ^ 2");

            var a = result.GetVariable("a");
            var b = result.GetVariable("b");

            Assert.AreEqual(5, a.Value.Number);
            Assert.AreEqual(25, b.Value.Number);
        }

        [TestMethod]
        public void DivNumConstantVariable()
        {
            var result = TestExecutor.Execute("a = 2", "b = 6 ^ a");

            var a = result.GetVariable("a");
            var b = result.GetVariable("b");

            Assert.AreEqual(2, a.Value.Number);
            Assert.AreEqual(36, b.Value.Number);
        }

        [TestMethod]
        public void Truncation()
        {
            Assert.AreEqual(0.008m, TestExecutor.Execute("a = 5 ^ -3").GetVariable("a").Value.Number);
            Assert.AreEqual(-0.008m, TestExecutor.Execute("a = -5 ^ -3").GetVariable("a").Value.Number);
            Assert.AreEqual(165.809m, TestExecutor.Execute("a = 5.2 ^ 3.1").GetVariable("a").Value.Number);
        }
    }
}

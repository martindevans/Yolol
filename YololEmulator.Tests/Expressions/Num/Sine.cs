using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YololEmulator.Tests.Expressions.Num
{
    [TestClass]
    public class Sine
    {
        [TestMethod]
        public void Sin0()
        {
            var result = TestExecutor.Execute("a = sin(0)");
            var a = result.GetVariable("a");
            Assert.AreEqual(0, (int)a.Value.Number);
        }

        [TestMethod]
        public void Sin45()
        {
            var result = TestExecutor.Execute("a = sin(45)");
            var a = result.GetVariable("a");
            Assert.AreEqual(0.707m, (decimal)a.Value.Number);
        }

        [TestMethod]
        public void Sin90()
        {
            var result = TestExecutor.Execute("a = sin(90)");
            var a = result.GetVariable("a");
            Assert.AreEqual(1, (int)a.Value.Number);
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YololEmulator.Tests.Expressions.Num
{
    [TestClass]
    public class Cosine
    {
        [TestMethod]
        public void Cos0()
        {
            var result = TestExecutor.Execute("a = cos(0)");
            var a = result.GetVariable("a");
            Assert.AreEqual(1, (int)a.Value.Number);
        }

        [TestMethod]
        public void Cos45()
        {
            var result = TestExecutor.Execute("a = cos(45)");
            var a = result.GetVariable("a");
            Assert.AreEqual(0.707m, (decimal)a.Value.Number);
        }

        [TestMethod]
        public void Cos90()
        {
            var result = TestExecutor.Execute("a = cos(90)");
            var a = result.GetVariable("a");
            Assert.AreEqual(0, (int)a.Value.Number);
        }
    }
}

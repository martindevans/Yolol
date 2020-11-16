using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Expressions.Num
{
    [TestClass]
    public class Tangent
    {
        [TestMethod]
        public void Tan0()
        {
            var result = TestExecutor.Execute("a = tan(0)");
            var a = result.GetVariable("a");
            Assert.AreEqual(0, (int)a.Value.Number);
        }

        [TestMethod]
        public void Tan45()
        {
            var result = TestExecutor.Execute("a = tan(45)");
            var a = result.GetVariable("a");
            Assert.AreEqual(1, (int)a.Value.Number);
        }

        [TestMethod]
        public void Tan90()
        {
            var result = TestExecutor.Execute("a = tan(90)");
            var a = result.GetVariable("a");
            Assert.AreEqual(Number.MaxValue, a.Value.Number);
        }
    }
}

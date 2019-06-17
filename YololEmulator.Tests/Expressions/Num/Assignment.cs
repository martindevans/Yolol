using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YololEmulator.Tests.Expressions.Num
{
    [TestClass]
    public class Assignment
    {
        [TestMethod]
        public void Positive()
        {
            var result = TestExecutor.Execute("a=1");

            var a = result.Get("a");

            Assert.AreEqual(1, a.Value.Number);
        }

        [TestMethod]
        public void Negative()
        {
            var result = TestExecutor.Execute("a=-1");

            var a = result.Get("a");

            Assert.AreEqual(-1, a.Value.Number);
        }

        [TestMethod]
        public void Precise()
        {
            var result = TestExecutor.Execute("a=1.11111");

            var a = result.Get("a");

            Assert.AreEqual(1.1111m, a.Value.Number);
        }

        [TestMethod]
        public void PrecisePrecise()
        {
            var result = TestExecutor.Execute("a=1.11111 b=1.1111 c=a==b");

            var c = result.Get("c");

            Assert.AreEqual(1, c.Value.Number);
        }

        [TestMethod]
        public void PrecisePreciseRounding()
        {
            var result = TestExecutor.Execute("a=1.11117 b=1.1111 c=a==b");

            var c = result.Get("c");

            Assert.AreEqual(1, c.Value.Number);
        }
    }
}

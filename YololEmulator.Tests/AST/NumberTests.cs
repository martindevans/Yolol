using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.AST
{
    [TestClass]
    public class NumberTests
    {
        [TestMethod]
        public void ZeroNumber()
        {
            var n = (Number)0.0m;

            Assert.AreEqual("0", n.ToString());
        }

        [TestMethod]
        public void ExtraPreciseNumber()
        {
            var n = (Number)17.0m;

            Assert.AreEqual("17", n.ToString());
        }

        [TestMethod]
        public void FullPreciseNumber()
        {
            var n = (Number)17.123m;

            Assert.AreEqual("17.123", n.ToString());
        }

        [TestMethod]
        public void BigNumber()
        {
            var n = (Number)17123m;

            Assert.AreEqual("17123", n.ToString());
        }

        [TestMethod]
        public void TinyNumber()
        {
            var n = (Number)0.001m;

            Assert.AreEqual("0.001", n.ToString());
        }
    }
}

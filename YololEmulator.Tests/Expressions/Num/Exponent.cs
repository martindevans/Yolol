using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

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
        public void Artturi()
        {
            // Results from the actual game, provided by Artturi
            // https://discordapp.com/channels/590281942184755239/593510152406171707/619082319487172608

            Number Calc(string code) => TestExecutor.Execute(code).GetVariable("a").Value.Number;

            Assert.AreEqual(0.008m, Calc("a = 5 ^ -3"));
            Assert.AreEqual(-0.008m, Calc("a = -5 ^ -3"));
            Assert.AreEqual(165.809m, Calc("a = 5.2 ^ 3.1"));
            Assert.AreEqual(0.006m, Calc("a = 5.2 ^ -3.1"));

            Assert.AreEqual(Number.MinValue, Calc("a = -1 ^ -0.5"));
        }
    }
}

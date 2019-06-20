using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YololEmulator.Tests.Expressions.Num
{
    [TestClass]
    public class Arithmetic
    {
        [TestMethod]
        public void NoBrackets()
        {
            var result = TestExecutor.Execute(
                "a = 1 + 2 * 4 / 8",
                "b = 1 + 4 / 8 * 2",
                "c = 4 / 8 * 2 + 1"
            );

            var a = result.GetVariable("a");
            var b = result.GetVariable("b");
            var c = result.GetVariable("c");

            const decimal expected = 1 + 2 * 4 / 8m;

            Assert.AreEqual(expected, a.Value.Number);
            Assert.AreEqual(expected, b.Value.Number);
            Assert.AreEqual(expected, c.Value.Number);
        }

        [TestMethod]
        public void Brackets()
        {
            var result = TestExecutor.Execute(
                "a = (2 + 2) * 4 / 8"
            );

            var a = result.GetVariable("a");

            const decimal expected = (2 + 2) * 4 / 8m;

            Assert.AreEqual(expected, a.Value.Number);
        }

        [TestMethod]
        public void Compound()
        {
            var result = TestExecutor.Execute(
                "a = 2",
                "a *= 3 + 4",
                "b = 2",
                "b = b * 3 + 4"
            );

            var a = result.GetVariable("a");
            var b = result.GetVariable("b");

            Assert.AreEqual(14, a.Value.Number);
            Assert.AreEqual(10, b.Value.Number);
        }
    }
}

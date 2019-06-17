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

            var a = result.Get("a");
            var b = result.Get("b");
            var c = result.Get("c");

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

            var a = result.Get("a");

            const decimal expected = (2 + 2) * 4 / 8m;

            Assert.AreEqual(expected, a.Value.Number);
        }
    }
}

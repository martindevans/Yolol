using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YololEmulator.Tests.Scripts.Basic
{
    [TestClass]
    public class Goto
    {
        [TestMethod]
        public void SkipLine()
        {
            var result = TestExecutor.Execute("a = 1", "goto 4", "a = 2", "b = 1 c = 1");

            var a = result.Get("a");
            var b = result.Get("b");
            var c = result.Get("c");

            Assert.AreEqual(1, a.Value.Number);
            Assert.AreEqual(1, b.Value.Number);
            Assert.AreEqual(1, c.Value.Number);
        }
    }
}

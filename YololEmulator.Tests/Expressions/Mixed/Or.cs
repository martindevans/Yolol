using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YololEmulator.Tests.Expressions.Mixed
{
    [TestClass]
    public class Or
    {
        [TestMethod]
        public void StringNumber()
        {
            var result = TestExecutor.Execute("a = 1 or \"2\"");

            var a = result.GetVariable("a");

            Assert.AreEqual(1, (int)a.Value.Number);
        }

        [TestMethod]
        public void NumberString()
        {
            var result = TestExecutor.Execute("a = \"1\" or 2");

            var a = result.GetVariable("a");

            Assert.AreEqual(1, (int)a.Value.Number);
        }
    }
}

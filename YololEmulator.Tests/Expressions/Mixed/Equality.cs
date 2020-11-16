using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YololEmulator.Tests.Expressions.Mixed
{
    [TestClass]
    public class Equality
    {
        [TestMethod]
        public void StringNumberFalse()
        {
            var result = TestExecutor.Execute("a = 1 == \"1\"");

            var a = result.GetVariable("a");

            Assert.AreEqual(0, (int)a.Value.Number);
        }

        [TestMethod]
        public void NumberStringFalse()
        {
            var result = TestExecutor.Execute("a = \"1\" == 1");

            var a = result.GetVariable("a");

            Assert.AreEqual(0, (int)a.Value.Number);
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Expressions.Mixed
{
    [TestClass]
    public class Addition
    {
        [TestMethod]
        public void StringNumber()
        {
            var result = TestExecutor.Execute("a = 1 + \"2\"");

            var a = result.GetVariable("a");

            Assert.AreEqual(new YString("12"), a.Value.String);
        }

        [TestMethod]
        public void NumberString()
        {
            var result = TestExecutor.Execute("a = \"1\" + 2");

            var a = result.GetVariable("a");

            Assert.AreEqual(new YString("12"), a.Value.String);
        }
    }
}

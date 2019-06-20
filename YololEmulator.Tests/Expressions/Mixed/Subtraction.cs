using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YololEmulator.Tests.Expressions.Mixed
{
    [TestClass]
    public class Subtraction
    {
        [TestMethod]
        public void NumberString()
        {
            var result = TestExecutor.Execute("a = 22 - \"2\"");

            var a = result.GetVariable("a");

            Assert.AreEqual("2", a.Value.String);
        }

        [TestMethod]
        public void NumberString_NothingRemoved()
        {
            var result = TestExecutor.Execute("a = 22 - \"1\"");

            var a = result.GetVariable("a");

            Assert.AreEqual("22", a.Value.String);
        }

        [TestMethod]
        public void StringNumber()
        {
            var result = TestExecutor.Execute("a = \"22\" - 2");

            var a = result.GetVariable("a");

            Assert.AreEqual("2", a.Value.String);
        }

        [TestMethod]
        public void StringNumber_NothingRemoved()
        {
            var result = TestExecutor.Execute("a = \"22\" - 1");

            var a = result.GetVariable("a");

            Assert.AreEqual("22", a.Value.String);
        }
    }
}

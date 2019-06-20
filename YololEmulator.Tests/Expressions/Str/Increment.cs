using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YololEmulator.Tests.Expressions.Str
{
    [TestClass]
    public class Increment
    {
        [TestMethod]
        public void PostIncrement()
        {
            var result = TestExecutor.Execute("a = \"a\"", "b = a++");

            var a = result.GetVariable("a");
            var b = result.GetVariable("b");

            Assert.AreEqual("a ", a.Value.String);
            Assert.AreEqual("a", b.Value.String);
        }

        [TestMethod]
        public void PreIncrement()
        {
            var result = TestExecutor.Execute("a = \"a\"", "b = ++a");

            var a = result.GetVariable("a");
            var b = result.GetVariable("b");

            Assert.AreEqual("a ", a.Value.String);
            Assert.AreEqual("a ", b.Value.String);
        }
    }
}

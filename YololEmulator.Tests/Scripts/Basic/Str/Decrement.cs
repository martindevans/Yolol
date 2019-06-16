using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YololEmulator.Tests.Scripts.Basic.Str
{
    [TestClass]
    public class Decrement
    {
        [TestMethod]
        public void PostDecrement()
        {
            var result = TestExecutor.Execute("a = \"ab\"", "b = a--");

            var a = result.Get("a");
            var b = result.Get("b");

            Assert.AreEqual("a", a.Value.String);
            Assert.AreEqual("ab", b.Value.String);
        }

        [TestMethod]
        public void PreDecrement()
        {
            var result = TestExecutor.Execute("a = \"ab\"", "b = --a");

            var a = result.Get("a");
            var b = result.Get("b");

            Assert.AreEqual("a", a.Value.String);
            Assert.AreEqual("a", b.Value.String);
        }
    }
}

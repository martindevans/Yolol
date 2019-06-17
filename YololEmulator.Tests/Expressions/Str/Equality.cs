using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YololEmulator.Tests.Expressions.Str
{
    [TestClass]
    public class Equality
    {
        [TestMethod]
        public void ConstantConstant()
        {
            var result = TestExecutor.Execute("a = \"2\" == \"2\"");

            var a = result.Get("a");

            Assert.AreEqual(1, a.Value.Number);
        }

        [TestMethod]
        public void VariableConstant()
        {
            var result = TestExecutor.Execute("a = \"2\" b = a == \"2\"");

            var a = result.Get("a");
            var b = result.Get("b");

            Assert.AreEqual("2", a.Value.String);
            Assert.AreEqual(1, b.Value.Number);
        }

        [TestMethod]
        public void VariableVariable()
        {
            var result = TestExecutor.Execute("a = \"2\" b = \"2\" c = b == a");

            var a = result.Get("a");
            var b = result.Get("b");
            var c = result.Get("c");

            Assert.AreEqual("2", a.Value.String);
            Assert.AreEqual("2", b.Value.String);
            Assert.AreEqual(1, c.Value.Number);
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YololEmulator.Tests.Scripts.Basic.Str
{
    [TestClass]
    public class Addition
    {
        [TestMethod]
        public void ConstantConstant()
        {
            var result = TestExecutor.Execute("a = \"a\" + \"b\"");

            var a = result.Get("a");

            Assert.AreEqual("ab", a.Value.String);
        }

        [TestMethod]
        public void VariableConstant()
        {
            var result = TestExecutor.Execute("a = \"a\"", "b = a + \"b\"");

            var a = result.Get("a");
            var b = result.Get("b");

            Assert.AreEqual("a", a.Value.String);
            Assert.AreEqual("ab", b.Value.String);
        }

        [TestMethod]
        public void ConstantVariable()
        {
            var result = TestExecutor.Execute("a = \"a\"", "b = \"b\" + a");

            var a = result.Get("a");
            var b = result.Get("b");

            Assert.AreEqual("a", a.Value.String);
            Assert.AreEqual("ba", b.Value.String);
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YololEmulator.Tests.Expressions.Str
{
    [TestClass]
    public class Subtraction
    {
        [TestMethod]
        public void ConstantConstant()
        {
            var result = TestExecutor.Execute("a = \"abc\" - \"b\"");

            var a = result.Get("a");

            Assert.AreEqual("ac", a.Value.String);
        }

        [TestMethod]
        public void ConstantConstant_NothingRemoved()
        {
            var result = TestExecutor.Execute("a = \"abc\" - \"d\"");

            var a = result.Get("a");

            Assert.AreEqual("abc", a.Value.String);
        }

        [TestMethod]
        public void VariableConstant()
        {
            var result = TestExecutor.Execute("a = \"abc\"", "b = a - \"b\"");

            var a = result.Get("a");
            var b = result.Get("b");

            Assert.AreEqual("abc", a.Value.String);
            Assert.AreEqual("ac", b.Value.String);
        }

        [TestMethod]
        public void ConstantVariable()
        {
            var result = TestExecutor.Execute("a = \"b\"", "b = \"abc\" - a");

            var a = result.Get("a");
            var b = result.Get("b");

            Assert.AreEqual("b", a.Value.String);
            Assert.AreEqual("ac", b.Value.String);
        }
    }
}

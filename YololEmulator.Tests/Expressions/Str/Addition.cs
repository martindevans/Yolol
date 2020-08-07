using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Expressions.Str
{
    [TestClass]
    public class Addition
    {
        [TestMethod]
        public void ConstantConstant()
        {
            var result = TestExecutor.Execute("a = \"ad\" + \"bc\"");

            var a = result.GetVariable("a");

            Assert.AreEqual(new YString("adbc"), a.Value.String);
        }

        [TestMethod]
        public void VariableConstant()
        {
            var result = TestExecutor.Execute("a = \"a\"", "b = a + \"b\"");

            var a = result.GetVariable("a");
            var b = result.GetVariable("b");

            Assert.AreEqual(new YString("a"), a.Value.String);
            Assert.AreEqual(new YString("ab"), b.Value.String);
        }

        [TestMethod]
        public void ConstantVariable()
        {
            var result = TestExecutor.Execute("a = \"a\"", "b = \"b\" + a");

            var a = result.GetVariable("a");
            var b = result.GetVariable("b");

            Assert.AreEqual(new YString("a"), a.Value.String);
            Assert.AreEqual(new YString("ba"), b.Value.String);
        }
    }
}

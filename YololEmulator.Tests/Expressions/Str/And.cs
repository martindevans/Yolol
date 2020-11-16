using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Expressions.Str
{
    [TestClass]
    public class And
    {
        [TestMethod]
        public void ConstantConstant()
        {
            var result = TestExecutor.Execute("a = \"a\" and \"b\"");

            var a = result.GetVariable("a");

            Assert.AreEqual(1, (int)a.Value.Number);
        }

        [TestMethod]
        public void VariableConstant()
        {
            var result = TestExecutor.Execute("a = \"a\"", "b = a and \"b\"");

            var a = result.GetVariable("a");
            var b = result.GetVariable("b");

            Assert.AreEqual(new YString("a"), a.Value.String);
            Assert.AreEqual(1, (int)b.Value.Number);
        }

        [TestMethod]
        public void ConstantVariable()
        {
            var result = TestExecutor.Execute("a = \"a\"", "b = \"b\" and a");

            var a = result.GetVariable("a");
            var b = result.GetVariable("b");

            Assert.AreEqual(new YString("a"), a.Value.String);
            Assert.AreEqual(1, (int)b.Value.Number);
        }
    }
}

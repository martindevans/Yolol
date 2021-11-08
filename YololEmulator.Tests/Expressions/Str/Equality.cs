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

            var a = result.GetVariable("a");

            Assert.AreEqual(1, (int)a.Value.Number);
        }

        [TestMethod]
        public void ConstantConstantFalse()
        {
            var result = TestExecutor.Execute("a = \"AAA\" == \"aaa\"");

            var a = result.GetVariable("a");

            Assert.AreEqual(0, (int)a.Value.Number);
        }

        [TestMethod]
        public void VariableConstant()
        {
            var result = TestExecutor.Execute("a = \"2\" b = a == \"2\"");

            var a = result.GetVariable("a");
            var b = result.GetVariable("b");

            Assert.AreEqual("2", a.Value.String.ToString());
            Assert.AreEqual(1, (int)b.Value.Number);
        }

        [TestMethod]
        public void VariableVariable()
        {
            var result = TestExecutor.Execute("a = \"2\" b = \"2\" c = b == a");

            var a = result.GetVariable("a");
            var b = result.GetVariable("b");
            var c = result.GetVariable("c");

            Assert.AreEqual("2", a.Value.String.ToString());
            Assert.AreEqual("2", b.Value.String.ToString());
            Assert.AreEqual(1, (int)c.Value.Number);
        }
    }
}

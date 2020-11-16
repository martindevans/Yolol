using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YololEmulator.Tests.Expressions.Str
{
    [TestClass]
    public class Not
    {
        [TestMethod]
        public void NotConstant()
        {
            var result = TestExecutor.Execute("a = not \"a\"");

            var a = result.GetVariable("a");

            Assert.AreEqual(0, (int)a.Value.Number);
        }
    }
}
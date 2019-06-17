using Microsoft.VisualStudio.TestTools.UnitTesting;
using YololEmulator.Execution;

namespace YololEmulator.Tests.Expressions.Mixed
{
    [TestClass]
    public class Multiplication
    {
        [TestMethod]
        public void StringNumber()
        {
            Assert.ThrowsException<ExecutionException>(() => {
                TestExecutor.Execute("a = 1 * \"2\"");
            });
        }

        [TestMethod]
        public void NumberString()
        {
            Assert.ThrowsException<ExecutionException>(() => {
                TestExecutor.Execute("a = \"1\" * 2");
            });
        }
    }
}

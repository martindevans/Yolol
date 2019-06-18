using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Expressions.Mixed
{
    [TestClass]
    public class Division
    {
        [TestMethod]
        public void StringNumber()
        {
            Assert.ThrowsException<ExecutionException>(() => {
                TestExecutor.Execute("a = 1 / \"2\"");
            });
        }

        [TestMethod]
        public void NumberString()
        {
            Assert.ThrowsException<ExecutionException>(() => {
                TestExecutor.Execute("a = \"1\" / 2");
            });
        }
    }
}

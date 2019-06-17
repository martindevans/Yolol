using Microsoft.VisualStudio.TestTools.UnitTesting;
using YololEmulator.Execution;

namespace YololEmulator.Tests.Expressions.Str
{
    [TestClass]
    public class Negate
    {
        [TestMethod]
        public void Invalid()
        {
            Assert.ThrowsException<ExecutionException>(() => {
                TestExecutor.Execute("a = \"str\" b = -a");
            });
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Expressions.Str
{
    [TestClass]
    public class Division
    {
        [TestMethod]
        public void Divide()
        {
            Assert.ThrowsException<ExecutionException>(() => {
                TestExecutor.Execute("a = \"a\" / \"b\"");
            });
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YololEmulator.Tests.Scripts
{
    [TestClass]
    public class ParserTorture
    {
        [TestMethod]
        public void MethodName()
        {
            var ast = TestExecutor.Parse(":d=1 goto (:a--)");
        }
    }
}

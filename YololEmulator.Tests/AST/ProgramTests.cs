using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YololEmulator.Tests.AST
{
    [TestClass]
    public class ProgramTests
    {
        [TestMethod]
        public void CommentLine()
        {
            var ast = TestExecutor.Parse("// a = b");

            Assert.AreEqual(0, ast.Lines.Count);
        }
    }
}

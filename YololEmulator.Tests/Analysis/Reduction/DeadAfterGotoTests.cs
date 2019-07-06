using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Analysis.TreeVisitor.Reduction;

namespace YololEmulator.Tests.Analysis.Reduction
{
    [TestClass]
    public class DeadAfterGotoTests
    {
        [TestMethod]
        public void BasicElimination()
        {
            var ast = TestExecutor.Parse("a=1 goto 2 a=3");
            var reduced = ast.DeadPostGotoElimination().ToString();
            Assert.AreEqual("a=1 goto 2", reduced);
        }

        [TestMethod]
        public void GotoInsideIfIfElimination()
        {
            var ast = TestExecutor.Parse("a=1 if :b then goto 2 a=4 else a=3 end");
            var reduced = ast.DeadPostGotoElimination().ToString();
            Assert.AreEqual("a=1 if :b then goto 2 end a=3", reduced);
        }

        [TestMethod]
        public void GotoInTrueBranch()
        {
            var ast = TestExecutor.Parse("if :b then goto 2 else a=3 end");
            var reduced = ast.DeadPostGotoElimination().ToString();
            Assert.AreEqual("if :b then goto 2 end a=3", reduced);
        }
    }
}

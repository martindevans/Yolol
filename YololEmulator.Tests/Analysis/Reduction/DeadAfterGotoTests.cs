using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Analysis.TreeVisitor.Reduction;

namespace YololEmulator.Tests.Analysis.Reduction
{
    [TestClass]
    public class DeadAfterGotoTests
    {
        private static ReducerTestHelper helper = new ReducerTestHelper(ast => ast.DeadPostGotoElimination());

        [TestMethod]
        public void BasicElimination() => helper.Run("a=1 goto 2 a=3", "a=1 goto 2");

        [TestMethod]
        public void GotoAfterGotoElimination() => helper.Run("a=1 goto 2 goto 3 a=3", "a=1 goto 2");

        [TestMethod]
        public void GotoInsideIfIfElimination() => helper.Run("a=1 if :b then goto 2 a=4 else a=3 end", "a=1 if :b then goto 2 end a=3");

        [TestMethod]
        public void GotoInTrueBranch() => helper.Run("if :b then goto 2 else a=3 end", "if :b then goto 2 end a=3");
    }
}

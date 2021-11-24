using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Analysis.TreeVisitor.Reduction;

namespace YololEmulator.Tests.Analysis.Reduction
{
    [TestClass]
    public class DeadPostGotoEliminationTests
    {
        private static readonly ReducerTestHelper Helper = new(ast => ast.DeadPostGotoElimination());

        [TestMethod]
        public void BasicElimination() => Helper.Run("a=1 goto 2 a=3", "a=1 goto 2");

        [TestMethod]
        public void GotoAfterGotoElimination() => Helper.Run("a=1 goto 2 goto 3 a=3", "a=1 goto 2");

        [TestMethod]
        public void GotoInsideIfIfElimination() => Helper.Run("a=1 if :b then goto 2 a=4 else a=3 end", "a=1 if :b then goto 2 end a=3");

        [TestMethod]
        public void GotoInTrueBranch() => Helper.Run("if :b then goto 2 else a=3 end", "if :b then goto 2 end a=3");
    }
}

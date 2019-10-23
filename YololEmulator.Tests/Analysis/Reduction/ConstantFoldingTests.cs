using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Analysis.TreeVisitor.Reduction;
using Yolol.Grammar;

namespace YololEmulator.Tests.Analysis.Reduction
{
    [TestClass]
    public class ConstantFoldingTests
    {
        private static ReducerTestHelper helper = new ReducerTestHelper(ast => ast.FoldConstants());

        [TestMethod]
        public void FoldNumber() => helper.Run("a=-2+(2*3)/2", "a=1");

        [TestMethod]
        public void FoldString() => helper.Run("a=\"sss\"+\"qqq\"", "a=\"sssqqq\"");

        [TestMethod]
        public void DoNotFoldExternals() => helper.Run("a=:extern+3", "a=:extern+3");

        [TestMethod]
        public void FoldIfTrue() => helper.Run("if 1 then a = 1 else a = 2 end", "a=1");

        [TestMethod]
        public void FoldIfFalse() => helper.Run("if 0 then a = 1 else a = 2 end", "a=2");

        [TestMethod]
        public void DoNotFoldIf() => helper.Run("if :extern then a = 1 else a = 2 end", "if :extern then a=1 else a=2 end");

        [TestMethod]
        public void DoNotFoldIfError() => helper.Run("if \"err\" then a = 1 else a = 2 end", "if \"err\" then a=1 else a=2 end");
    }
}

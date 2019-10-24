using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Analysis.TreeVisitor.Reduction;

namespace YololEmulator.Tests.Analysis.Reduction
{
    [TestClass]
    public class ConstantFoldingTests
    {
        private static readonly ReducerTestHelper Helper = new ReducerTestHelper(ast => ast.FoldConstants());

        [TestMethod]
        public void FoldNumber() => Helper.Run("a=-2+(2*3)/2", "a=1");

        [TestMethod]
        public void FoldString() => Helper.Run("a=\"sss\"+\"qqq\"", "a=\"sssqqq\"");

        [TestMethod]
        public void DoNotFoldExternals() => Helper.Run("a=:extern+3", "a=:extern+3");

        [TestMethod]
        public void FoldIfTrue() => Helper.Run("if 1 then a = 1 else a = 2 end", "a=1");

        [TestMethod]
        public void FoldIfFalse() => Helper.Run("if 0 then a = 1 else a = 2 end", "a=2");

        [TestMethod]
        public void DoNotFoldIf() => Helper.Run("if :extern then a = 1 else a = 2 end", "if :extern then a=1 else a=2 end");

        [TestMethod]
        public void DoNotFoldIfError() => Helper.Run("if \"err\" then a = 1 else a = 2 end", "if \"err\" then a=1 else a=2 end");
    }
}

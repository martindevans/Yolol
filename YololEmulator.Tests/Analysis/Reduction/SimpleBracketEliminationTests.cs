using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Analysis.TreeVisitor.Reduction;

namespace YololEmulator.Tests.Analysis.Reduction
{
    [TestClass]
    public class SimpleBracketEliminationTests
    {
        private static readonly ReducerTestHelper Helper = new(ast => ast.SimpleBracketElimination());

        [TestMethod]
        public void NumBrackets() => Helper.Run("a=(1)*2+3", "a=1*2+3");

        [TestMethod]
        public void NumNoBrackets() => Helper.Run("a=1*2+3", "a=1*2+3");

        [TestMethod]
        public void StrBrackets() => Helper.Run("a=(\"1\")*2+3", "a=\"1\"*2+3");

        [TestMethod]
        public void StrNoBrackets() => Helper.Run("a=\"1\"*2+3", "a=\"1\"*2+3");

        [TestMethod]
        public void AssignmentBrackets() => Helper.Run("a=(1*2+3)", "a=1*2+3");

        [TestMethod]
        public void AssignmentNoBrackets() => Helper.Run("a=1*2+3", "a=1*2+3");

        [TestMethod]
        public void Gotobrackets() => Helper.Run("goto (1*2+3)", "goto 1*2+3");

        [TestMethod]
        public void GotoNoBrackets() => Helper.Run("goto 1*2+3", "goto 1*2+3");

        [TestMethod]
        public void Ifbrackets() => Helper.Run("if (1*2+3) then else end", "if 1*2+3 then else end");

        [TestMethod]
        public void IfNoBrackets() => Helper.Run("if 1*2+3 then else end", "if 1*2+3 then else end");
    }
}

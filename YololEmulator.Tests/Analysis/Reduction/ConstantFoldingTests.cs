using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Analysis.Reduction;
using Yolol.Grammar;

namespace YololEmulator.Tests.Analysis.Reduction
{
    [TestClass]
    public class ConstantFoldingTests
    {
        [TestMethod]
        public void FoldNumber()
        {
            var ast = Parser.TryParseProgram(Tokenizer.TryTokenize("a=-2+(2*3)/2").Value).Value;
            var reduced = ast.FoldConstants().ToString();
            Assert.AreEqual("a=1", reduced);
        }

        [TestMethod]
        public void FoldString()
        {
            var ast = Parser.TryParseProgram(Tokenizer.TryTokenize("a=\"sss\"+\"qqq\"").Value).Value;
            var reduced = ast.FoldConstants().ToString();
            Assert.AreEqual("a=\"sssqqq\"", reduced);
        }

        [TestMethod]
        public void DoNotFoldExternals()
        {
            var ast = Parser.TryParseProgram(Tokenizer.TryTokenize("a=:extern+3").Value).Value;
            var reduced = ast.FoldConstants().ToString();
            Assert.AreEqual("a=:extern+3", reduced);
        }

        [TestMethod]
        public void FoldIfTrue()
        {
            var ast = Parser.TryParseProgram(Tokenizer.TryTokenize("if 1 then a = 1 else a = 2 end").Value).Value;
            var reduced = ast.FoldConstants().ToString();
            Assert.AreEqual("a=1", reduced);
        }

        [TestMethod]
        public void FoldIfFalse()
        {
            var ast = Parser.TryParseProgram(Tokenizer.TryTokenize("if 0 then a = 1 else a = 2 end").Value).Value;
            var reduced = ast.FoldConstants().ToString();
            Assert.AreEqual("a=2", reduced);
        }

        [TestMethod]
        public void DoNotFoldIf()
        {
            var ast = Parser.TryParseProgram(Tokenizer.TryTokenize("if :extern then a = 1 else a = 2 end").Value).Value;
            var reduced = ast.FoldConstants().ToString();
            Assert.AreEqual("if :extern then a=1 else a=2 end", reduced);
        }

        [TestMethod]
        public void DoNotFoldIfError()
        {
            var ast = Parser.TryParseProgram(Tokenizer.TryTokenize("if \"err\" then a = 1 else a = 2 end").Value).Value;
            var reduced = ast.FoldConstants().ToString();
            Assert.AreEqual("if \"err\" then a=1 else a=2 end", reduced);
        }
    }
}

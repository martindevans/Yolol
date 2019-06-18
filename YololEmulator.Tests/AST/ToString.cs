using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Grammar;

namespace YololEmulator.Tests.AST
{
    [TestClass]
    public class ToString
    {
        private void Roundtrip(string line)
        {
            var tok = Tokenizer.TryTokenize(line);
            Assert.IsTrue(tok.HasValue);
            var par = Parser.TryParse(tok.Value);
            Assert.IsTrue(par.HasValue);

            Assert.AreEqual(line, par.Value.ToString());
        }

        [TestMethod]
        public void If()
        {
            Roundtrip("if a==1 then end");
        }

        [TestMethod]
        public void IfTrue()
        {
            Roundtrip("if a==1 then a=1 end");
        }

        [TestMethod]
        public void IfTrueFalse()
        {
            Roundtrip("if a==1 then a=1 else a=2 end");
        }

        [TestMethod]
        public void Goto()
        {
            Roundtrip("goto 7");
        }

        [TestMethod]
        public void Assignment()
        {
            Roundtrip("a=17");
        }

        [TestMethod]
        public void AssignmentVar()
        {
            Roundtrip("a=b");
        }

        [TestMethod]
        public void String()
        {
            Roundtrip("a=\"hi\"");
        }

        [TestMethod]
        public void Number()
        {
            Roundtrip("a=17");
        }

        [TestMethod]
        public void Negate()
        {
            Roundtrip("a=-17");
        }

        [TestMethod]
        public void PostDec()
        {
            Roundtrip("a--");
        }

        [TestMethod]
        public void PreDec()
        {
            Roundtrip("--a");
        }

        [TestMethod]
        public void PostInc()
        {
            Roundtrip("a++");
        }

        [TestMethod]
        public void PreInc()
        {
            Roundtrip("++a");
        }
    }
}

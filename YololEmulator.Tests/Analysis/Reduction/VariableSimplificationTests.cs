using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Analysis.Reduction;
using Yolol.Grammar;

namespace YololEmulator.Tests.Analysis.Reduction
{
    [TestClass]
    public class VariableSimplificationTests
    {
        [TestMethod]
        public void SimplifyVarNames()
        {
            // Parse the initial (complex) code
            var ast = Parser.TryParseProgram(Tokenizer.TryTokenize("ship=7\niff=0 ship=6\n").Value).Value;

            // Reduce AST to a simpler program
            var reduced = ast.SimplifyVariableNames().ToString();

            // Check we got the simpler program we expected
            Assert.AreEqual("a=7\nb=0 a=6\n", reduced);
        }

        [TestMethod]
        public void PreserveExternalNames()
        {
            // Parse the initial (complex) code
            var ast = Parser.TryParseProgram(Tokenizer.TryTokenize("ship=7 :counter=0 ship=6").Value).Value;

            // Reduce AST to a simpler program
            var reduced = ast.SimplifyVariableNames().ToString();

            // Check we got the simpler program we expected
            Assert.AreEqual("a=7 :counter=0 a=6", reduced);
        }

        [TestMethod]
        public void LotsOfUniqueNames()
        {
            var n = new SequentialNameGenerator();
            var h = new HashSet<string>();
            for (var i = 0; i < 1000; i++)
                Assert.IsTrue(h.Add(n.Name()));
        }
    }
}

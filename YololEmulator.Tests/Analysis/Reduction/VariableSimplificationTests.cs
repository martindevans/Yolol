using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Analysis;
using Yolol.Analysis.TreeVisitor.Reduction;
using Yolol.Grammar;

namespace YololEmulator.Tests.Analysis.Reduction
{
    [TestClass]
    public class VariableSimplificationTests
    {
        private static ReducerTestHelper helper = new ReducerTestHelper(ast => ast.SimplifyVariableNames());

        [TestMethod]
        public void SimplifyVarNames() => helper.Run("ship=7\nixf=0 ship=6", "a=7\nb=0 a=6");

        [TestMethod]
        public void PreserveExternalNames() => helper.Run("ship=7 :counter=0 ship=6", "a=7 :counter=0 a=6");

        [TestMethod]
        public void LotsOfUniqueNames()
        {
            var n = new SequentialNameGenerator("");
            var h = new HashSet<string>();
            for (var i = 0; i < 1000; i++)
                Assert.IsTrue(h.Add(n.Name()));
        }
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Analysis.TreeVisitor.Reduction;

namespace YololEmulator.Tests.TreeVisitor
{
    [TestClass]
    public class OpByConstNumCompressorTests
    {
        [TestMethod]
        public void And()
        {
            var ast1 = TestExecutor.Parse("a = \"2\" and 1");
            var ast2 = new OpNumByConstNumCompressor(null).Visit(ast1);

            Console.WriteLine(ast2);
        }
    }
}

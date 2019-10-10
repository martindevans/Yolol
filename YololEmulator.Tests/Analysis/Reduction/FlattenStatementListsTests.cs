using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Analysis.TreeVisitor.Reduction;
using Yolol.Grammar;
using Yolol.Grammar.AST;
using Yolol.Grammar.AST.Expressions.Unary;
using Yolol.Grammar.AST.Statements;

namespace YololEmulator.Tests.Analysis.Reduction
{
    [TestClass]
    public class FlattenStatementListsTests
    {
        [TestMethod]
        public void Flatten()
        {
            var ast = new Program(new Line[] {
                new Line(new StatementList(new Assignment(new VariableName("a"), new Sine(new Variable(new VariableName("b"))))))
            });

            var r = new FlattenStatementLists().Visit(ast).ToString();

            Assert.AreEqual("a=sin(b)", r);
        }
    }
}

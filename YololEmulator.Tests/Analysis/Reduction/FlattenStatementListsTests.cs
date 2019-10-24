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
        private static ReducerTestHelper helper = new ReducerTestHelper(ast => new FlattenStatementLists().Visit(ast));

        [TestMethod]
        public void Flatten() => helper.Run("a=sin(b)", new Program(new Line[] {
            new Line(new StatementList(new Assignment(new VariableName("a"), new Sine(new Variable(new VariableName("b"))))))
        }));
    }
}

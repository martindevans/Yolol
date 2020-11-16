using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;
using Yolol.Grammar;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Statements;

namespace YololEmulator.Tests.AST
{
    [TestClass]
    public class EmptyStatementTests
    {
        [TestMethod]
        public void Equality()
        {
            var a = new EmptyStatement();
            var b = new EmptyStatement();

            Assert.IsTrue(a.Equals((BaseStatement)b));
        }

        [TestMethod]
        public void Inequality()
        {
            var a = new EmptyStatement();
            var b = new Assignment(new VariableName("a"), new ConstantNumber((Number)3));

            Assert.IsFalse(a.Equals((BaseStatement)b));
        }
    }
}

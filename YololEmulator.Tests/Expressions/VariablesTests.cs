using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Expressions.Unary;

namespace YololEmulator.Tests.Expressions
{
    [TestClass]
    public class VariablesTests
    {
        [TestMethod]
        public void IsNotBoolean()
        {
            Assert.IsFalse(new Variable(new Yolol.Grammar.VariableName("a")).IsBoolean);
        }
    }
}

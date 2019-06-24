using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Grammar;
using Yolol.Grammar.AST.Expressions.Unary;

namespace YololEmulator.Tests.Expressions
{
    [TestClass]
    public class ModifyTests
    {
        [TestMethod]
        public void IsNotConstant()
        {
            var app = new PreIncrement(new VariableName("abc"));
            Assert.IsFalse(app.IsConstant);
        }
    }
}

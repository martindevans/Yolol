using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Grammar.AST.Expressions;

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

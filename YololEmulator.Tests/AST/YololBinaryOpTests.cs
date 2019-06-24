using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Grammar;

namespace YololEmulator.Tests.AST
{
    [TestClass]
    public class YololBinaryOpTests
    {
        [TestMethod]
        public void MethodName()
        {
            Assert.AreEqual("+", YololBinaryOp.Add.String());
            Assert.AreEqual("/", YololBinaryOp.Divide.String());
            Assert.AreEqual("==", YololBinaryOp.EqualTo.String());
            Assert.AreEqual("^", YololBinaryOp.Exponent.String());
            Assert.AreEqual(">", YololBinaryOp.GreaterThan.String());
            Assert.AreEqual(">=", YololBinaryOp.GreaterThanEqualTo.String());
            Assert.AreEqual("<", YololBinaryOp.LessThan.String());
            Assert.AreEqual("<=", YololBinaryOp.LessThanEqualTo.String());
            Assert.AreEqual("%", YololBinaryOp.Modulo.String());
            Assert.AreEqual("*", YololBinaryOp.Multiply.String());
            Assert.AreEqual("!=", YololBinaryOp.NotEqualTo.String());
            Assert.AreEqual("-", YololBinaryOp.Subtract.String());
        }
    }
}

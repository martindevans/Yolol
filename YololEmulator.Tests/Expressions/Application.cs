using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;
using Yolol.Grammar;
using Yolol.Grammar.AST.Expressions.Unary;

namespace YololEmulator.Tests.Expressions
{
    [TestClass]
    public class Application
    {
        [TestMethod]
        public void CanNotRuntimeError()
        {
            var app = new Yolol.Grammar.AST.Expressions.Unary.Application(new FunctionName("func"), new ConstantNumber(1));
            Assert.IsFalse(app.CanRuntimeError);
        }

        [TestMethod]
        public void CallInSequence()
        {
            var result = TestExecutor.Execute("a = 7*cos(0)");
            var a = result.GetVariable("a");
            Assert.AreEqual(7, a.Value.Number);
        }

        [TestMethod]
        public void CallUnknown()
        {
            Assert.ThrowsException<ExecutionException>(() =>
            {
                TestExecutor.Execute("a = unknown(1)");
            });
        }

        [TestMethod]
        public void ExpressionParameter()
        {
            var result = TestExecutor.Execute("a = cos(89+1)");
            var a = result.GetVariable("a");
            Assert.AreEqual(0, a.Value.Number);
        }

        [TestMethod]
        public void InExpression()
        {
            var result = TestExecutor.Execute("a=cos(90)-45");
            var a = result.GetVariable("a");
            Assert.AreEqual(-45, a.Value.Number);
        }

        [TestMethod]
        public void IsConstant()
        {
            var app = new Yolol.Grammar.AST.Expressions.Unary.Application(new FunctionName("func"), new ConstantNumber(3));
            Assert.IsTrue(app.IsConstant);
        }

        [TestMethod]
        public void IsNotConstant()
        {
            var app = new Yolol.Grammar.AST.Expressions.Unary.Application(new FunctionName("func"), new Yolol.Grammar.AST.Expressions.Unary.Variable(new VariableName("var")));
            Assert.IsFalse(app.IsConstant);
        }
    }
}

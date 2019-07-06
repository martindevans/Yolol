using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;
using Yolol.Grammar.AST.Expressions.Binary;
using Yolol.Grammar.AST.Expressions.Unary;
using Yolol.Grammar.AST.Statements;

namespace YololEmulator.Tests.Statements
{
    [TestClass]
    public class GotoTests
    {
        [TestMethod]
        public void Constant()
        {
            var result = TestExecutor.Execute("a = 1", "goto 4", "a = 2", "b = 1 c = 1");

            var a = result.GetVariable("a");
            var b = result.GetVariable("b");
            var c = result.GetVariable("c");

            Assert.AreEqual(1, a.Value.Number);
            Assert.AreEqual(1, b.Value.Number);
            Assert.AreEqual(1, c.Value.Number);
        }

        [TestMethod]
        public void Variable()
        {
            var result = TestExecutor.Execute("a = 4", "goto a", "a = 2", "b = 1 c = 1");

            var a = result.GetVariable("a");
            var b = result.GetVariable("b");
            var c = result.GetVariable("c");

            Assert.AreEqual(4, a.Value.Number);
            Assert.AreEqual(1, b.Value.Number);
            Assert.AreEqual(1, c.Value.Number);
        }

        [TestMethod]
        public void String()
        {
            Assert.ThrowsException<ExecutionException>(() => {
                TestExecutor.Execute("goto \"hello\"");
            });
        }

        [TestMethod]
        public void CanRuntimeError()
        {
            Assert.IsTrue(new Goto(new ConstantString("a")).CanRuntimeError);
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;
using Yolol.Grammar;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Expressions.Binary;
using Yolol.Grammar.AST.Statements;
using Variable = Yolol.Grammar.AST.Expressions.Variable;

namespace YololEmulator.Tests.Statements
{
    [TestClass]
    public class IfTests
    {
        [TestMethod]
        public void IfNone()
        {
            var result = TestExecutor.Execute("a = 1", "b = 2 if a == 1 then else end");

            var a = result.GetVariable("a");

            Assert.AreEqual(1, (int)a.Value.Number);
        }

        [TestMethod]
        public void IfTrue()
        {
            var result = TestExecutor.Execute("a = 1", "if a == 1 then a = 2 end");

            var a = result.GetVariable("a");

            Assert.AreEqual(2, (int)a.Value.Number);
        }

        [TestMethod]
        public void IfFalse()
        {
            var result = TestExecutor.Execute("a = 1", "if a == 2 then a = 2 end");

            var a = result.GetVariable("a");

            Assert.AreEqual(1, (int)a.Value.Number);
        }

        [TestMethod]
        public void IfElseTrue()
        {
            var result = TestExecutor.Execute("a = 1", "if a == 1 then a = 2 else a = 3 end");

            var a = result.GetVariable("a");

            Assert.AreEqual(2, (int)a.Value.Number);
        }

        [TestMethod]
        public void IfElseFalse()
        {
            var result = TestExecutor.Execute("a = 1", "if a == 2 then a = 2 else a = 3 end");

            var a = result.GetVariable("a");

            Assert.AreEqual(3, (int)a.Value.Number);
        }

        [TestMethod]
        public void IfSkipGoto()
        {
            var result = TestExecutor.Execute(
                "a = 1",
                "if a == 1 then goto 4 end",
                "a = 2"
            );

            var a = result.GetVariable("a");
            Assert.AreEqual(1, (int)a.Value.Number);
        }

        [TestMethod]
        public void CanRuntimeErrorFalse()
        {
            var i = new If(new Variable(new VariableName("a")), new StatementList(), new StatementList());

            Assert.IsFalse(i.CanRuntimeError);
        }

        [TestMethod]
        public void CanRuntimeErrorTrueCon()
        {
            var i = new If(new Divide(new ConstantNumber((Number)1), new ConstantNumber((Number)0)), new StatementList(), new StatementList());

            Assert.IsTrue(i.CanRuntimeError);
        }

        [TestMethod]
        public void CanRuntimeErrorTrueLeft()
        {
            var i = new If(new Variable(new VariableName("a")), new StatementList(new Assignment(new VariableName("a"), new Divide(new ConstantNumber((Number)1), new ConstantNumber((Number)0)))), new StatementList());

            Assert.IsTrue(i.CanRuntimeError);
        }

        [TestMethod]
        public void CanRuntimeErrorTrueRight()
        {
            var i = new If(new Variable(new VariableName("a")), new StatementList(), new StatementList(new Assignment(new VariableName("a"), new Divide(new ConstantNumber((Number)1), new ConstantNumber((Number)0)))));

            Assert.IsTrue(i.CanRuntimeError);
        }
    }
}

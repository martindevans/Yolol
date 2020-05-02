using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;
using Yolol.Grammar;
using Yolol.Grammar.AST;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Expressions.Binary;
using Yolol.Grammar.AST.Expressions.Unary;
using Yolol.Grammar.AST.Statements;
using Variable = Yolol.Grammar.AST.Expressions.Variable;

namespace YololEmulator.Tests.AST
{
    [TestClass]
    public class Precedence
    {
        // These tests each test an operator binds more strongly than the item immediately below it in this table:
        // https://github.com/Jerald/yolol-is-cylons/blob/master/information/cylon_yolol.md

        [TestMethod]
        public void Negate()
        {
            var actual = TestExecutor.Parse("a = -b * c");
            var expect = new Line(new StatementList(new Assignment(new VariableName("a"), new Multiply(new Negate(new Variable(new VariableName("b"))), new Variable(new VariableName("c"))))));

            Assert.IsTrue(expect.Equals(actual.Lines[0]));
        }

        [TestMethod]
        public void Increment()
        {
            var actual = TestExecutor.Parse("a = -b++");
            var expect = new Line(new StatementList(new Assignment(new VariableName("a"), new Negate(new PostIncrement(new VariableName("b"))))));

            Assert.IsTrue(expect.Equals(actual.Lines[0]));
        }

        [TestMethod]
        public void Keyword()
        {
            var actual = TestExecutor.Parse("a = sqrt b ^ c");
            var expect = new Line(new StatementList(new Assignment(new VariableName("a"), new Exponent(new Sqrt(new Variable(new VariableName("b"))), new Variable(new VariableName("c"))))));

            Assert.IsTrue(expect.Equals(actual.Lines[0]));
        }

        [TestMethod]
        public void ExponentAssociativity()
        {
            var state = TestExecutor.Execute("a = 2 ^ 2 ^ 3");
            Assert.AreEqual(256, state.GetVariable("a").Value.Number);
        }

        [TestMethod]
        public void ExponentMul()
        {
            var state = TestExecutor.Execute("a = 2 ^ 3 * 4");
            Assert.AreEqual(32, state.GetVariable("a").Value.Number);
        }

        [TestMethod]
        public void ExponentDiv()
        {
            var state = TestExecutor.Execute("a = 2 ^ 3 / 4");
            Assert.AreEqual(2, state.GetVariable("a").Value.Number);
        }

        [TestMethod]
        public void ExponentMod()
        {
            var state = TestExecutor.Execute("a = 2 ^ 3 % 5");
            Assert.AreEqual(3, state.GetVariable("a").Value.Number);
        }

        [TestMethod]
        public void MultiplyDiv()
        {
            var state = TestExecutor.Execute("a = 8 / 4 * 2");
            Assert.AreEqual(4, state.GetVariable("a").Value.Number);
        }

        [TestMethod]
        public void MultiplyDiv2()
        {
            var state = TestExecutor.Execute("a = 10000 * 12.345 / 10000");
            Assert.AreEqual(10m, state.GetVariable("a").Value.Number);
        }

        [TestMethod]
        public void MultiplyAdd()
        {
            var state = TestExecutor.Execute("a = 2 * 3 + 4");
            Assert.AreEqual(10, state.GetVariable("a").Value.Number);
        }

        [TestMethod]
        public void MultiplySub()
        {
            var state = TestExecutor.Execute("a = 2 * 3 - 4");
            Assert.AreEqual(2, state.GetVariable("a").Value.Number);
        }

        [TestMethod]
        public void AddLt()
        {
            var state = TestExecutor.Execute("a = 2 + 3 < 4");
            Assert.AreEqual(0, state.GetVariable("a").Value.Number);
        }

        [TestMethod]
        public void AddGt()
        {
            var state = TestExecutor.Execute("a = 2 + 3 > 4");
            Assert.AreEqual(1, state.GetVariable("a").Value.Number);
        }

        [TestMethod]
        public void AddLtEq()
        {
            var state = TestExecutor.Execute("a = 2 + 3 <= 4");
            Assert.AreEqual(0, state.GetVariable("a").Value.Number);
        }

        [TestMethod]
        public void AddGtEq()
        {
            var state = TestExecutor.Execute("a = 2 + 3 >= 4");
            Assert.AreEqual(1, state.GetVariable("a").Value.Number);
        }

        [TestMethod]
        public void GtEq()
        {
            var state = TestExecutor.Execute("a = 2 > 4 == 4");
            Assert.AreEqual(0, state.GetVariable("a").Value.Number);
        }

        [TestMethod]
        public void EqOr()
        {
            var state = TestExecutor.Execute("a = 4 == 4 or 0");
            Assert.AreEqual(1, state.GetVariable("a").Value.Number);
        }

        [TestMethod]
        public void AndOr()
        {
            var actual = TestExecutor.Parse("a = 1 and 2 or 3");
            var expect = new Line(new StatementList(new Assignment(new VariableName("a"), new And(new ConstantNumber(1), new Or(new ConstantNumber(2), new ConstantNumber(3))))));

            Assert.IsTrue(expect.Equals(actual.Lines[0]));
        }
    }
}

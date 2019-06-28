using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Analysis.Reduction;
using Yolol.Execution;
using Yolol.Grammar.AST.Statements;

namespace YololEmulator.Tests.Analysis.Reduction
{
    [TestClass]
    public class IfAssignmentCompressionTests
    {
        [TestMethod] public void TrueBranchIsTrue()
        {
            var (m1, m2) = TestExecutor.Equivalence(
                a => a.CompressConditionalAssignment(),
                "if a == 0 then b = 5 end"
            );

            Assert.AreEqual(m1.GetVariable("b").Value, m2.GetVariable("b").Value);
        }

        [TestMethod] public void TrueBranchIsFalse()
        {
            var (m1, m2) = TestExecutor.Equivalence(
                a => a.CompressConditionalAssignment(),
                "if a == 1 then b = 5 end"
            );

            Assert.AreEqual(m1.GetVariable("b").Value, m2.GetVariable("b").Value);
        }

        [TestMethod] public void DualBranchIsTrue()
        {
            var (m1, m2) = TestExecutor.Equivalence(
                a => a.CompressConditionalAssignment(),
                "b = 3 if a == 0 then b = 5 else b = 10 end"
            );

            Assert.AreEqual(m1.GetVariable("b").Value, m2.GetVariable("b").Value);
        }

        [TestMethod] public void DualBranchIsFalse()
        {
            var (m1, m2) = TestExecutor.Equivalence(
                a => a.CompressConditionalAssignment(),
                "b = 3 if a == 1 then b = 5 else b = 10 end"
            );
            
            Assert.AreEqual(m1.GetVariable("b").Value, m2.GetVariable("b").Value);
        }

        [TestMethod]
        public void Playground()
        {
            var ast = TestExecutor.Parse(
                "if a == 18 then x = 11 end",
                "if a == 18 then z = 1 else z = 2 end"
            );
            Console.WriteLine(ast);
            Console.WriteLine();

            Console.WriteLine(ast.CompressConditionalAssignment());
        }

        [TestMethod]
        public void SanityCheck()
        {
            var result = TestExecutor.Execute("b = 12 if a == 1 then b=17 end");
            Assert.AreEqual(0, result.GetVariable("a").Value.Number);
            Assert.AreEqual(12, result.GetVariable("b").Value.Number);

            var ast = TestExecutor.Parse(
                "b = 12 if a == 1 then b=17 end"
            );

            ast = ast.CompressConditionalAssignment();

            var state = new MachineState(new ConstantNetwork(), new DefaultIntrinsics());
            ast.Lines.Single().Evaluate(0, state);

            Assert.AreEqual(0, state.GetVariable("a").Value.Number);
            Assert.AreEqual(12, state.GetVariable("b").Value.Number);
        }

        [TestMethod]
        public void DoesNotAffectStrings()
        {
            var ast = TestExecutor.Parse("b = 12 if a == 1 then b=\"17\" end");

            var ast2 = ast.CompressConditionalAssignment();

            Assert.AreEqual(ast.ToString(), ast2.ToString());
        }

        [TestMethod]
        public void DoesNotAffectComplexIf()
        {
            var ast = TestExecutor.Parse("b = 12 if a == 1 then c=1 b=2 end");

            var ast2 = ast.CompressConditionalAssignment();

            Assert.AreEqual(ast.ToString(), ast2.ToString());
        }
    }
}

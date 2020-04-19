using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Z3;
using Yolol.Analysis.ControlFlowGraph.Extensions;
using Yolol.Analysis.SAT;
using Yolol.Analysis.TreeVisitor.Reduction;
using Yolol.Analysis.Types;
using Yolol.Execution;
using Yolol.Grammar;

namespace YololEmulator.Tests.SAT
{
    [TestClass]
    public class BuilderTests
    {
        private IModel BuildModel(string line)
        {
            var ast = TestExecutor.Parse(line);

            var cfg = new Yolol.Analysis.ControlFlowGraph.Builder(ast, 1).Build();
            cfg = cfg.StaticSingleAssignment(out var ssa);
            cfg = cfg.FlowTypingAssignment(ssa, out var types);
            cfg = cfg.TypeDrivenEdgeTrimming(types);
            cfg = cfg.MergeAdjacentBasicBlocks();
            cfg = cfg.VisitBlocks(t => new OpNumByConstNumCompressor(t), types);
            cfg = cfg.VisitBlocks(() => new ErrorCompressor());

            return cfg.Vertices.First(x => x.LineNumber == 1 && x.Statements.Any()).BuildSAT(types);
        }

        private void AssertTainted(IModel sat, string name, Yolol.Execution.Type type)
        {
            Assert.AreEqual(Status.SATISFIABLE, sat.Check());

            foreach (var item in sat.Solver.Model.Consts)
            {
                var vv = sat.Solver.Model.Eval(item.Value);
                Console.WriteLine($"{item.Key.Name} = {vv}");
            }

            var a = sat.TryGetVariable(new VariableName(name));

            Assert.IsNotNull(a, "variable is null");
            Assert.IsFalse(a.IsValueAvailable(), "value is unavailable (tainted)");

            Assert.AreEqual(type == Yolol.Execution.Type.Number, a.CanBeNumber(), "type");
            Assert.AreEqual(type == Yolol.Execution.Type.String, a.CanBeString(), "type");
        }

        private void AssertValue(IModel sat, string name, Value v, bool exact = true)
        {
            var var = sat.TryGetVariable(new VariableName(name));
            AssertValue(sat, var, v, exact);
        }

        private void AssertValue(IModel sat, IModelVariable var, Value v, bool exact = true)
        {
            Assert.AreEqual(Status.SATISFIABLE, sat.Check());

            foreach (var item in  sat.Solver.Model.Consts)
            {
                var vv = sat.Solver.Model.Eval(item.Value);
                Console.WriteLine($"{item.Key.Name} = {vv}");
            }

            var a = var;

            Assert.IsNotNull(a, "variable is null");
            Assert.IsTrue(a.IsValueAvailable(), "value is unavailable (tainted)");

            Assert.AreEqual(v.Type == Yolol.Execution.Type.Number, a.CanBeNumber(), "type");
            Assert.AreEqual(v.Type == Yolol.Execution.Type.String, a.CanBeString(), "type");

            // The expected value can't possible equal all these values, so we can check if it's _not_ these values
            var aString = new Value("abc");
            var bString = new Value("cba");
            var aNum = new Value(19);
            var bNum = new Value(20);

            if (!v.Equals(aString)) Assert.IsFalse(a.CanBeValue(aString), "Can be string it should not be");
            if (!v.Equals(bString)) Assert.IsFalse(a.CanBeValue(bString), "Can be string it should not be");
            if (!v.Equals(aNum)) Assert.IsFalse(a.CanBeValue(aNum), "Can be num it should not be");
            if (!v.Equals(bNum)) Assert.IsFalse(a.CanBeValue(bNum), "Can be num it should not be");

            Assert.IsTrue(a.CanBeValue(v), "Can be value");

            if (exact)
                Assert.IsTrue(a.IsValue(v), "Is value");
        }

        [TestMethod]
        public void AssignmentToConstantNumber()
        {
            var sat = BuildModel("a = 1");
            AssertValue(sat, "a[0]", 1);
        }

        [TestMethod]
        public void AssignmentToConstantString()
        {
            var sat = BuildModel("a = \"1\"");
            AssertValue(sat, "a[0]", "1");
        }

        [TestMethod]
        public void AssignmentToVariable()
        {
            var sat = BuildModel("a = \"1\" b = a");
            AssertValue(sat, "b[0]", "1");
        }

        [TestMethod]
        public void StringStringAddition()
        {
            var sat = BuildModel("a = \"2\" + \"1\"");
            AssertValue(sat, "a[0]", "21");
        }

        [TestMethod]
        public void StringNumAddition()
        {
            var sat = BuildModel("a = \"a\" + 2");

            Assert.AreEqual(Status.SATISFIABLE, sat.Check());

            var a = sat.TryGetVariable(new VariableName("a[0]"));

            Assert.IsNotNull(a);
            Assert.IsFalse(a.IsValueAvailable());
            Assert.IsFalse(a.CanBeNumber());
            Assert.IsTrue(a.CanBeString());
            //Assert.IsFalse(a.CanBeValue(new Value(new Number(1))));
            //Assert.IsFalse(a.CanBeValue(new Value("2")));
            //Assert.IsTrue(a.IsValue(new Value("a2")));
        }

        [TestMethod]
        public void NumStringAddition()
        {
            var sat = BuildModel("a = 2 + \"a\"");
            Assert.AreEqual(Status.SATISFIABLE, sat.Check());

            var a = sat.TryGetVariable(new VariableName("a[0]"));

            Assert.IsNotNull(a);
            Assert.IsFalse(a.IsValueAvailable());
            Assert.IsFalse(a.CanBeNumber());
            Assert.IsTrue(a.CanBeString());
            //Assert.IsFalse(a.CanBeValue(new Value(new Number(1))));
            //Assert.IsFalse(a.CanBeValue(new Value("2")));
            //Assert.IsTrue(a.IsValue(new Value("2a")));
        }

        [TestMethod]
        public void NumNumAddition()
        {
            var sat = BuildModel("a = 2 + 3");
            AssertValue(sat, "a[0]", 5);
        }

        [TestMethod]
        public void StringStringSubtraction()
        {
            var sat = BuildModel("a = \"2\" - \"1\" b = \"31\" - \"1\"");
            AssertValue(sat, "a[0]", "2");
            AssertValue(sat, "b[0]", "3");
        }

        [TestMethod]
        public void StringNumSubtraction()
        {
            var sat = BuildModel("a = \"a\" - 2");

            Assert.AreEqual(Microsoft.Z3.Status.SATISFIABLE, sat.Check());

            var a = sat.TryGetVariable(new VariableName("a[0]"));

            Assert.IsNotNull(a);
            Assert.IsFalse(a.IsValueAvailable());
            Assert.IsFalse(a.CanBeNumber());
            Assert.IsTrue(a.CanBeString());
            //Assert.IsFalse(a.CanBeValue(new Value(new Number(1))));
            //Assert.IsFalse(a.CanBeValue(new Value("2")));
            //Assert.IsTrue(a.IsValue(new Value("a2")));
        }

        [TestMethod]
        public void NumStringSubtraction()
        {
            var sat = BuildModel("a = 2 - \"a\"");
            Assert.AreEqual(Microsoft.Z3.Status.SATISFIABLE, sat.Check());

            var a = sat.TryGetVariable(new VariableName("a[0]"));

            Assert.IsNotNull(a);
            Assert.IsFalse(a.IsValueAvailable());
            Assert.IsFalse(a.CanBeNumber());
            Assert.IsTrue(a.CanBeString());
            //Assert.IsFalse(a.CanBeValue(new Value(new Number(1))));
            //Assert.IsFalse(a.CanBeValue(new Value("2")));
            //Assert.IsTrue(a.IsValue(new Value("2a")));
        }

        [TestMethod]
        public void NumNumSubtraction()
        {
            var sat = BuildModel("a = 2 - 3");
            AssertValue(sat, "a[0]", -1);
        }

        [TestMethod]
        public void NumNumMultiplication()
        {
            var sat = BuildModel("a = 2 * 3");
            AssertValue(sat, "a[0]", 6);
        }

        [TestMethod]
        public void NumNumDivision()
        {
            var sat = BuildModel("a = 6 / 3");
            AssertValue(sat, "a[0]", 2);
        }

        [TestMethod]
        public void NumNumDivisionDecimal()
        {
            var sat = BuildModel("a = 3 / 6");
            AssertValue(sat, "a[0]", 0.5m);
        }

        [TestMethod]
        public void StringStringEquality()
        {
            var sat = BuildModel("a = \"2\" == \"1\" b = \"a\" == \"a\"");
            AssertValue(sat, "a[0]", 0);
            AssertValue(sat, "b[0]", 1);
        }

        [TestMethod]
        public void StringNumEquality()
        {
            var sat = BuildModel("a = \"2\" == 1 b = \"2\" == 2");

            AssertValue(sat, "a[0]", 0, false);
            AssertValue(sat, "a[0]", 1, false);

            AssertValue(sat, "b[0]", 0, false);
            AssertValue(sat, "b[0]", 1, false);
        }

        [TestMethod]
        public void NumStringEquality()
        {
            var sat = BuildModel("a = 2 == \"1\" b = 2 == \"2\"");

            AssertValue(sat, "a[0]", 0, false);
            AssertValue(sat, "a[0]", 1, false);

            AssertValue(sat, "b[0]", 0, false);
            AssertValue(sat, "b[0]", 1, false);
        }

        [TestMethod]
        public void NumNumEquality()
        {
            var sat = BuildModel("a = 2 == 3 b = 3 == 3");
            AssertValue(sat, "a[0]", 0);
            AssertValue(sat, "b[0]", 1);
        }

        [TestMethod]
        public void StringStringNonEquality()
        {
            var sat = BuildModel("a = \"2\" != \"1\" b = \"a\" != \"a\"");
            AssertValue(sat, "a[0]", 1);
            AssertValue(sat, "b[0]", 0);
        }

        [TestMethod]
        public void StringNumNonEquality()
        {
            var sat = BuildModel("a = \"2\" != 1 b = \"2\" != 2");

            AssertValue(sat, "a[0]", 1, false);
            AssertValue(sat, "a[0]", 0, false);

            AssertValue(sat, "b[0]", 1, false);
            AssertValue(sat, "b[0]", 0, false);
        }

        [TestMethod]
        public void NumStringNonEquality()
        {
            var sat = BuildModel("a = 2 != \"1\" b = 2 != \"2\"");

            AssertValue(sat, "a[0]", 1, false);
            AssertValue(sat, "a[0]", 0, false);

            AssertValue(sat, "b[0]", 1, false);
            AssertValue(sat, "b[0]", 0, false);
        }

        [TestMethod]
        public void NumNumNonEquality()
        {
            var sat = BuildModel("a = 2 != 3 b = 3 != 3");
            AssertValue(sat, "a[0]", 1);
            AssertValue(sat, "b[0]", 0);
        }

        [TestMethod]
        public void StringStringGreaterThan()
        {
            var sat = BuildModel("a = \"2\" > \"1\" b = \"a\" > \"b\"");
            AssertValue(sat, "a[0]", 0, false);
            AssertValue(sat, "a[0]", 1, false);

            AssertValue(sat, "b[0]", 0, false);
            AssertValue(sat, "b[0]", 1, false);
        }

        [TestMethod]
        public void StringNumLessGreaterThan()
        {
            var sat = BuildModel("a = \"2\" > 1 b = \"2\" > 3");

            AssertValue(sat, "a[0]", 1, false);
            AssertValue(sat, "a[0]", 0, false);

            AssertValue(sat, "b[0]", 1, false);
            AssertValue(sat, "b[0]", 0, false);
        }

        [TestMethod]
        public void NumStringGreaterThan()
        {
            var sat = BuildModel("a = 2 > \"1\" b = 2 > \"2\"");

            AssertValue(sat, "a[0]", 1, false);
            AssertValue(sat, "a[0]", 0, false);

            AssertValue(sat, "b[0]", 1, false);
            AssertValue(sat, "b[0]", 0, false);
        }

        [TestMethod]
        public void NumNumGreaterThan()
        {
            var sat = BuildModel("a = 2 > 2 b = 4 > 3");
            AssertValue(sat, "a[0]", 0);
            AssertValue(sat, "b[0]", 1);
        }

        [TestMethod]
        public void StringStringGreaterThanEqualTo()
        {
            var sat = BuildModel("a = \"2\" >= \"1\" b = \"a\" >= \"b\"");
            AssertValue(sat, "a[0]", 0, false);
            AssertValue(sat, "a[0]", 1, false);

            AssertValue(sat, "b[0]", 0, false);
            AssertValue(sat, "b[0]", 1, false);
        }

        [TestMethod]
        public void StringNumLessGreaterThanEqualTo()
        {
            var sat = BuildModel("a = \"2\" >= 1 b = \"2\" >= 3");

            AssertValue(sat, "a[0]", 1, false);
            AssertValue(sat, "a[0]", 0, false);

            AssertValue(sat, "b[0]", 1, false);
            AssertValue(sat, "b[0]", 0, false);
        }

        [TestMethod]
        public void NumStringGreaterThanEqualTo()
        {
            var sat = BuildModel("a = 2 >= \"1\" b = 2 >= \"2\"");

            AssertValue(sat, "a[0]", 1, false);
            AssertValue(sat, "a[0]", 0, false);

            AssertValue(sat, "b[0]", 1, false);
            AssertValue(sat, "b[0]", 0, false);
        }

        [TestMethod]
        public void NumNumGreaterThanEqualTo()
        {
            var sat = BuildModel("a = 2 >= 3 b = 4 >= 3");
            AssertValue(sat, "a[0]", 0);
            AssertValue(sat, "b[0]", 1);
        }

        [TestMethod]
        public void StringStringLessThan()
        {
            var sat = BuildModel("a = \"2\" < \"1\" b = \"a\" < \"b\"");
            AssertValue(sat, "a[0]", 0, false);
            AssertValue(sat, "a[0]", 1, false);

            AssertValue(sat, "b[0]", 0, false);
            AssertValue(sat, "b[0]", 1, false);
        }

        [TestMethod]
        public void StringNumLessThan()
        {
            var sat = BuildModel("a = \"2\" < 1 b = \"2\" < 3");

            AssertValue(sat, "a[0]", 1, false);
            AssertValue(sat, "a[0]", 0, false);

            AssertValue(sat, "b[0]", 1, false);
            AssertValue(sat, "b[0]", 0, false);
        }

        [TestMethod]
        public void NumStringLessThan()
        {
            var sat = BuildModel("a = 2 < \"1\" b = 2 < \"2\"");

            AssertValue(sat, "a[0]", 1, false);
            AssertValue(sat, "a[0]", 0, false);

            AssertValue(sat, "b[0]", 1, false);
            AssertValue(sat, "b[0]", 0, false);
        }

        [TestMethod]
        public void NumNumLessThan()
        {
            var sat = BuildModel("a = 2 < 2 b = 3 < 4");
            AssertValue(sat, "a[0]", 0);
            AssertValue(sat, "b[0]", 1);
        }

        [TestMethod]
        public void StringStringLessThanOrEqual()
        {
            var sat = BuildModel("a = \"2\" <= \"1\" b = \"a\" <= \"b\"");
            AssertValue(sat, "a[0]", 0, false);
            AssertValue(sat, "a[0]", 1, false);

            AssertValue(sat, "b[0]", 0, false);
            AssertValue(sat, "b[0]", 1, false);
        }

        [TestMethod]
        public void StringNumLessThanOrEqual()
        {
            var sat = BuildModel("a = \"2\" <= 1 b = \"2\" <= 2");

            AssertValue(sat, "a[0]", 1, false);
            AssertValue(sat, "a[0]", 0, false);

            AssertValue(sat, "b[0]", 1, false);
            AssertValue(sat, "b[0]", 0, false);
        }

        [TestMethod]
        public void NumStringLessThanOrEqual()
        {
            var sat = BuildModel("a = 2 <= \"1\" b = 2 <= \"2\"");

            AssertValue(sat, "a[0]", 1, false);
            AssertValue(sat, "a[0]", 0, false);

            AssertValue(sat, "b[0]", 1, false);
            AssertValue(sat, "b[0]", 0, false);
        }

        [TestMethod]
        public void NumNumLessThanOrEqual()
        {
            var sat = BuildModel("a = 2 <= 1 b = 3 <= 4");
            AssertValue(sat, "a[0]", 0);
            AssertValue(sat, "b[0]", 1);
        }

        [TestMethod]
        public void StringStringAnd()
        {
            var sat = BuildModel("a = \"2\" and \"1\"");
            AssertValue(sat, "a[0]", 1);
        }

        [TestMethod]
        public void StringNumAnd()
        {
            var sat = BuildModel("a = \"2\" and 1");

            AssertValue(sat, "a[0]", 1);
        }

        [TestMethod]
        public void NumStringAnd()
        {
            var sat = BuildModel("a = 2 and \"1\"");

            AssertValue(sat, "a[0]", 1);
        }

        [TestMethod]
        public void NumNumAnd()
        {
            var sat = BuildModel("a = 2 and 3 b = 3 and 0");
            AssertValue(sat, "a[0]", 1);
            AssertValue(sat, "b[0]", 0);
        }

        [TestMethod]
        public void StringStringOr()
        {
            var sat = BuildModel("a = \"2\" or \"1\"");
            AssertValue(sat, "a[0]", 1);
        }

        [TestMethod]
        public void StringNumOr()
        {
            var sat = BuildModel("a = \"2\" or 1");

            AssertValue(sat, "a[0]", 1);
        }

        [TestMethod]
        public void NumStringOr()
        {
            var sat = BuildModel("a = 2 or \"1\"");

            AssertValue(sat, "a[0]", 1);
        }

        [TestMethod]
        public void NumNumOr()
        {
            var sat = BuildModel("a = 2 or 3 b = 3 or 0 c = 0 or 0");
            AssertValue(sat, "a[0]", 1);
            AssertValue(sat, "b[0]", 1);
            AssertValue(sat, "c[0]", 0);
        }

        [TestMethod]
        public void NumNegate()
        {
            var sat = BuildModel("a = -2");
            AssertValue(sat, "a[0]", -2);
        }

        [TestMethod]
        public void StrNegate()
        {
            BuildModel("a = \"-2\"");
        }

        [TestMethod]
        public void NumSqrt()
        {
            var sat = BuildModel("a = sqrt(4)");
            AssertTainted(sat, "a[0]", Yolol.Execution.Type.Number);
        }

        [TestMethod]
        public void NumAbs()
        {
            var sat = BuildModel("a = abs(4) b = abs(-5)");
            AssertValue(sat, "a[0]", 4);
            AssertValue(sat, "b[0]", 5);
        }

        [TestMethod]
        public void NumSine()
        {
            var sat = BuildModel("a = Sin(90)");

            AssertValue(sat, "a[0]", -1, false);
            AssertValue(sat, "a[0]", 0, false);
            AssertValue(sat, "a[0]", 1, false);
        }

        [TestMethod]
        public void NumCosine()
        {
            var sat = BuildModel("a = Cos(90)");

            AssertValue(sat, "a[0]", -1, false);
            AssertValue(sat, "a[0]", 0, false);
            AssertValue(sat, "a[0]", 1, false);
        }

        [TestMethod]
        public void NumNot()
        {
            var sat = BuildModel("a = not 1 b = not 0");

            AssertValue(sat, "a[0]", 0);
            AssertValue(sat, "b[0]", 1);
        }

        [TestMethod]
        public void StrNot()
        {
            var sat = BuildModel("a = not \"a\"");

            AssertValue(sat, "a[0]", 0);
        }

        [TestMethod]
        public void Goto()
        {
            var sat = BuildModel("a = not 3 goto a");

            AssertValue(sat, sat.TryGetGotoVariable(), 0);
        }

        [TestMethod]
        public void ErrorExpression()
        {
            var sat = BuildModel("a = 0/0");

            Assert.IsNull(sat.TryGetVariable(new VariableName("a[0]")));
        }
    }
}

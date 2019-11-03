using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Analysis.ControlFlowGraph.Extensions;
using Yolol.Analysis.SAT;
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

            var cfg = new Yolol.Analysis.ControlFlowGraph.Builder(ast).Build();
            cfg = cfg.StaticSingleAssignment(out var ssa);
            cfg = cfg.FlowTypingAssignment(ssa, out var types);
            cfg = cfg.TypeDrivenEdgeTrimming(types);
            cfg = cfg.MergeAdjacentBasicBlocks();

            return cfg.Vertices.Single(x => x.LineNumber == 1 && x.Statements.Any()).BuildSAT(types);
        }

        private void AssertValue(IModel sat, string name, Value v, bool exact = true)
        {
            Assert.AreEqual(Microsoft.Z3.Status.SATISFIABLE, sat.Check());

            var a = sat.TryGetVariable(new VariableName(name));

            Assert.IsNotNull(a, "variable is null");
            Assert.IsTrue(a.IsValueAvailable(), "value is unavailable (tainted)");

            Assert.AreEqual(v.Type == Type.Number, a.CanBeNumber());
            Assert.AreEqual(v.Type == Type.String, a.CanBeString());

            // The expected value can't possible equal all these values, so we can check if it's _not_ these values
            var aString = new Value("abc");
            var bString = new Value("cba");
            var aNum = new Value(19);
            var bNum = new Value(20);

            if (v != aString) Assert.IsFalse(a.CanBeValue(aString), "Can be string it should not be");
            if (v != bString) Assert.IsFalse(a.CanBeValue(bString), "Can be string it should not be");
            if (v != aNum) Assert.IsFalse(a.CanBeValue(aNum), "Can be num it should not be");
            if (v != bNum) Assert.IsFalse(a.CanBeValue(bNum), "Can be num it should not be");

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

            Assert.AreEqual(Microsoft.Z3.Status.SATISFIABLE, sat.Check());

            var a = sat.TryGetVariable(new VariableName("b[0]"));

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
            Assert.AreEqual(Microsoft.Z3.Status.SATISFIABLE, sat.Check());

            var a = sat.TryGetVariable(new VariableName("b[0]"));

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
    }
}

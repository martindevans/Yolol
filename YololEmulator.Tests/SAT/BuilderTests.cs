using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Z3;
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
        [TestMethod]
        public void AssignmentToConstantNumber()
        {
            var ast = TestExecutor.Parse("a = 1");

            var cfg = new Yolol.Analysis.ControlFlowGraph.Builder(ast).Build();
            cfg = cfg.StaticSingleAssignment(out var ssa);
            cfg = cfg.FlowTypingAssignment(ssa, out var types);
            var sat = cfg.Vertices.Single(x => x.LineNumber == 1 && x.Statements.Any()).BuildSAT(types);

            var a = sat.TryGetVariable(new VariableName("a[0]"));

            Assert.IsNotNull(a);

            a.IsValue(new Value(new Number(1)));
        }

        [TestMethod]
        public void AssignmentToConstantString()
        {
            var ast = TestExecutor.Parse("a = \"1\"");

            var cfg = new Yolol.Analysis.ControlFlowGraph.Builder(ast).Build();
            cfg = cfg.StaticSingleAssignment(out var ssa);
            cfg = cfg.FlowTypingAssignment(ssa, out var types);
            var sat = cfg.Vertices.Single(x => x.LineNumber == 1 && x.Statements.Any()).BuildSAT(types);

            var a = sat.TryGetVariable(new VariableName("a[0]"));

            Assert.IsNotNull(a);

            a.IsValue(new Value("1"));
        }

        
    }
}

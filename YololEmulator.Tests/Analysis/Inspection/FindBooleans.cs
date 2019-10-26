using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yolol.Analysis.ControlFlowGraph.Extensions;
using Yolol.Analysis.TreeVisitor.Reduction;
using Yolol.Grammar;

namespace YololEmulator.Tests.Analysis.Inspection
{
    [TestClass]
    public class FindBooleans
    {
        [TestMethod]
        public void CountBooleans()
        {
            var ast = TestExecutor.Parse(
                "a=30<2 theAnswer=42 b=theAnswer==42"
            );

            var cfg = new Yolol.Analysis.ControlFlowGraph.Builder(ast.StripTypes()).Build();

            cfg = cfg.StaticSingleAssignment(out var ssa);

            var variableNames = cfg.FindBooleanVariables(ssa).ToHashSet();

            Assert.IsTrue(variableNames.SetEquals(new VariableName[] {
                new VariableName("c[0]"),
                new VariableName("a[0]"),
                new VariableName("f[0]"),
                new VariableName("b[0]")
                }));

            cfg.FindBooleanVariables(ssa).ToList().ForEach(x => Console.WriteLine(ssa.BaseName(x)));

            Console.WriteLine(cfg.RemoveUnreachableBlocks().MergeAdjacentBasicBlocks().ToDot());
        }
    }
}

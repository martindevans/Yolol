using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Analysis;
using Yolol.Analysis.ControlFlowGraph;
using Yolol.Analysis.ControlFlowGraph.Extensions;
using Yolol.Analysis.TreeVisitor;
using Yolol.Grammar;
using Yolol.Grammar.AST;

namespace YololEmulator.Tests.Optimisation
{
    [TestClass]
    public class BugReproduction
    {
        private async Task<(Program, Program)> Optimise(params string[] program)
        {
            return await Optimise(Array.Empty<(VariableName, Yolol.Execution.Type)>(), program);
        }

        private async Task<(Program, Program)> Optimise((VariableName, Yolol.Execution.Type)[] hints, params string[] program)
        {
            var ast = TestExecutor.Parse(program);

            Console.WriteLine("## Input");
            Console.WriteLine(ast);
            Console.WriteLine();

            var p = new OptimisationPipeline(ast.Lines.Count, 1, false, hints);
            var r = await p.Apply(ast);
            Console.WriteLine("## Output");
            Console.WriteLine(r);

            return (ast, r);
        }

        [TestMethod]
        public async Task AggressiveVariableElimination()
        { 
            var (a, b) = await Optimise("a = :a b = a a++ :c = a + b");

            var x = TestExecutor.Execute(a);
            var y = TestExecutor.Execute(b);

            Assert.AreEqual(1, x.GetVariable(":c").Value.Number);
            Assert.AreEqual(1, y.GetVariable(":c").Value.Number);
        }
    }
}

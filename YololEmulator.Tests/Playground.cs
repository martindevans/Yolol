using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Analysis;
using Yolol.Analysis.ControlFlowGraph;
using Yolol.Analysis.ControlFlowGraph.Extensions;
using Yolol.Analysis.TreeVisitor;
using Yolol.Analysis.TreeVisitor.Reduction;
using Yolol.Analysis.Types;
using Yolol.Grammar;
using Yolol.Grammar.AST.Statements;

namespace YololEmulator.Tests
{
    [TestClass]
    public class Playground
    {
        [TestMethod]
        public void CFG()
        {
            //var ast = TestExecutor.Parse(
            //    "a = :a b = :b",
            //    "c = a + b",
            //    "if a/2 > 10 then :c = 1/:a else :c = \"str\" end d = c",
            //    "goto 2"
            //);

            var ast = TestExecutor.Parse(
                "z = 2 a = :a * z a /= z",
                "flag=a==:a if flag then goto 5 else goto 6 end",
                "x = \"hello\" * 4 goto \"world\" x = 2",
                "b*=2 flag=b>30 if flag then :b=a end",
                "b=b-1 goto 4",
                "b=b+1 goto 4"
            );

            //var ast = TestExecutor.Parse(
            //    ":o1=0+(:a*1)+(:a/1)+:a^1+(:a-0)",
            //    ":o2=\"hello\"*1",
            //    ":o3=a/0",
            //    ":o4=a^\"world\"",
            //    "goto 1"
            //);

            //var ast = TestExecutor.Parse(
            //    "a = :a a *= 1 goto 3",
            //    "a++ goto 1",
            //    "b = a * 2 goto 1"
            //);
            Console.WriteLine(ast);
            Console.WriteLine();

            var cfg = new Builder(ast).Build();

            var hints = new[] {
                (new VariableName(":a"), Yolol.Execution.Type.Number)
            };

            // Find types
            cfg = cfg.StaticSingleAssignment(out var ssa);
            cfg = cfg.FlowTypingAssignment(ssa, out var types, hints);

            // Optimise graph based on types
            // ReSharper disable once AccessToModifiedClosure
            cfg = cfg.VisitBlocks(() => new OpNumByConstNumCompressor(types));
            cfg = cfg.VisitBlocks(() => new ErrorCompressor());
            cfg = cfg.VisitBlocks(u => new RemoveUnreadAssignments(u, ssa), c => c.FindUnreadAssignments());
            cfg = cfg.FlowTypingAssignment(ssa, out types, hints);
            cfg = cfg.TypeDrivenEdgeTrimming(types);
            cfg = cfg.NormalizeErrors();

            // Minify the graph
            for (var i = 0; i < 10; i++)
                cfg = cfg.RemoveEmptyNodes();
            cfg = cfg.RemoveUnreachableBlocks();

            // Convert optimised graph to dot
            var dot = cfg.ToDot();

            // Apply some simplifications before conversion into yolol
            cfg = cfg.RemoveStaticSingleAssignment(ssa);

            // Convert back into Yolol
            var yolol = cfg.ToYolol().StripTypes();
            Console.WriteLine(yolol);
            Console.WriteLine();

            Console.WriteLine(dot);
        }

        [TestMethod]
        public void ExprDecomposition()
        {
            var ast = TestExecutor.Parse(
                "a = b+c*-(d+z)%14+sin(3*2)+(y++)"
            );
            var ass = (Assignment)ast.Lines.Single().Statements.Statements.Single();

            var stmts = new ExpressionDecomposition(new SequentialNameGenerator("__tmp")).Visit(ass.Right);
            foreach (var stmt in stmts)
                Console.WriteLine(stmt);

            Console.WriteLine("a = " + ((Assignment)stmts.Last()).Left.Name);
        }

        [TestMethod]
        public void EStmtDecomposition()
        {
            var ast = TestExecutor.Parse(
                "if a-- then b *= 3 else c-- end",
                "c++ d-- goto 3",
                "q=q/z"
            );

            var prog = new ProgramDecomposition(new SequentialNameGenerator("__tmp")).Visit(ast);
            foreach (var line in prog.Lines)
                Console.WriteLine(line);
            
        }

        [TestMethod]
        public void PiCalculator()
        {
            var ast = TestExecutor.Parse(
                "r = 715237 zz=0.33333",
                "i = 0 A = 1664525    M = 2^32",
                "s = 0 C = 1013904223 F = 2^16",
                "r=(r*A+C)%M x=(r%F)/F r=(r*A+C)%M y=(r%F)/F",
                "s+=1 i+=(x*x+y*y)<1 pi=4*(i/s) goto 4"
            );

            Console.WriteLine(ast);
            Console.WriteLine($"Score: {ast.ToString().Length}");
            Console.WriteLine();
            
            ast = ast.FoldConstants();
            ast = ast.HoistConstants();
            ast = ast.CompressConstants();
            ast = ast.SimplifyVariableNames();
            ast = ast.DeadPostGotoElimination();
            ast = ast.CompressCompoundIncrement();

            Console.WriteLine(ast);
            Console.WriteLine($"Score: {ast.ToString().Length}");
        }
    }
}

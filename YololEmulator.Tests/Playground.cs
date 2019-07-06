using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Analysis;
using Yolol.Analysis.ControlFlowGraph;
using Yolol.Analysis.ControlFlowGraph.Extensions;
using Yolol.Analysis.TreeVisitor;
using Yolol.Analysis.TreeVisitor.Reduction;
using Yolol.Analysis.Types;
using Yolol.Grammar.AST.Statements;

namespace YololEmulator.Tests
{
    [TestClass]
    public class Playground
    {
        //[TestMethod]
        //public void MethodName()
        //{
        //    var l1 = "ship=:radio check=11111 if check != 0 then check = 22222 else check = 11111 end";
        //    var l2 = "gunnery_state = 33333 ciws_readyness_state = 0 general_alarm_state = 0";

        //    var l1p = Parser.TryParseLine(Tokenizer.TryTokenize(l1).Value).Value;
        //    Console.WriteLine(l1p);

        //    var l2p = Parser.TryParseLine(Tokenizer.TryTokenize(l2).Value).Value;
        //    Console.WriteLine(l2p);

        //    var prog = l1 + "\n" + l2;

        //    var tokens = Tokenizer.TryTokenize(prog);
        //    if (!tokens.HasValue)
        //        Console.WriteLine(tokens.FormatErrorMessageFragment());

        //    // Why does this not parse in multi line, but it does as two separate lines!?
        //    var astResult = Parser.TryParseProgram(tokens.Value);
        //    if (!astResult.HasValue)
        //        Console.WriteLine(astResult.FormatErrorMessageFragment());

        //    var ast = astResult.Value;
        //    Console.WriteLine(ast.ToString());
        //}

        [TestMethod]
        public void CFG()
        {
            var ast = TestExecutor.Parse(
                "a = :a b = :b",
                "c = a + b",
                "if a/2 > 10 then b = 1 else b = \"str\" end",
                "goto 2"
            );

            //var ast = TestExecutor.Parse(
            //    "z = 2 a = :a * z a /= z",
            //    "flag=a==:a if flag then goto 5 else goto 6 end",
            //    "x = \"hello\" * 4 goto \"world\" x = 2",
            //    "b*=2 flag=b>30 if flag then :b=a end",
            //    "b=b-1 goto 4",
            //    "b=b+1 goto 4"
            //);
            Console.WriteLine(ast);
            Console.WriteLine();

            var cfg = new Builder(ast).Build();

            // Minify the graph
            for (var i = 0; i < 10; i++)
                cfg = cfg.RemoveEmptyNodes();
            cfg = cfg.RemoveUnreachableBlocks();

            // Modify the graph
            cfg = cfg.StaticSingleAssignment(out var ssa);
            cfg = cfg.FlowTypingAssignment(ssa, out var types);

            var s = cfg.ToDot();
            Console.WriteLine(s);
        }

        [TestMethod]
        public void ExprDecomposition()
        {
            var ast = TestExecutor.Parse(
                "a = b+c*-(d+z)%14+sin(3*2)+(y++)"
            );
            var ass = ast.Lines.Single().Statements.Statements.Single() as Assignment;

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

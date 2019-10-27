using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Z3;
using Yolol.Analysis;
using Yolol.Analysis.TreeVisitor;
using Yolol.Analysis.TreeVisitor.Reduction;
using Yolol.Execution;
using Yolol.Grammar;
using Yolol.Grammar.AST.Statements;

namespace YololEmulator.Tests
{
    [TestClass]
    public class Playground
    {
        [TestMethod]
        public void Z3()
        {
            //z(Number)=-0.5==(:a)
            //ab(Number)=(z)
            //bb(Number)=(ab)
            //db(Number)=-((bb))
            //eb(Number)=6+db
            //goto eb

            using (var ctx = new Context())
            using (var solver = ctx.MkSolver())
            {
                var z = ctx.MkConst("z", ctx.IntSort);
                solver.Assert(ctx.MkOr(ctx.MkEq(z, ctx.MkInt(1000)), ctx.MkEq(z, ctx.MkInt(0000))));

                var ab = ctx.MkConst("ab", ctx.IntSort);
                solver.Assert(ctx.MkEq(ab, z));

                var bb = ctx.MkConst("bb", ctx.IntSort);
                solver.Assert(ctx.MkEq(bb, ab));

                var db = ctx.MkConst("db", ctx.IntSort);
                solver.Assert(ctx.MkEq(bb, ctx.MkMul(ctx.MkInt(-1), (IntExpr)db)));

                var eb = ctx.MkConst("eb", ctx.IntSort);
                solver.Assert(ctx.MkEq(eb, ctx.MkAdd(ctx.MkInt(6000), (IntExpr)db)));

                var @goto = (IntExpr)ctx.MkConst("goto", ctx.IntSort);

                var x = ctx.MkITE(ctx.MkLt((IntExpr)eb, ctx.MkInt(1000)), ctx.MkInt(1000), eb);
                x = ctx.MkITE(ctx.MkGt((IntExpr)x, ctx.MkInt(20000)), ctx.MkInt(20000), eb);
                x = ctx.MkDiv((IntExpr)x, ctx.MkInt(1000));
                solver.Assert(ctx.MkEq(@goto, x));

                while (solver.Check() == Status.SATISFIABLE)
                {
                    var v = (IntNum)solver.Model.Eval(@goto);
                    Console.WriteLine(v);

                    solver.Assert(ctx.MkNot(ctx.MkEq(@goto, v)));
                }

                Console.WriteLine(solver.Check());
            }
        }

        [TestMethod]
        public async Task CFG()
        {
            //var ast = TestExecutor.Parse(
            //    "a = :a b = :b",
            //    "c = a + b",
            //    "if a/2 > 10 then :c = 1/:a else :c = \"str\" end d = c",
            //    "goto 2"
            //);

            //var ast = TestExecutor.Parse(
            //    "char = :a",
            //    "min=0 max=10 search=5 k10=10000",
            //    "l1 = char >= search if l1 then min=search else max=search end search=min+((max-min)/2)/k10*k10",
            //    "l2 = char >= search if l2 then min=search else max=search end search=min+((max-min)/2)/k10*k10",
            //    "l3 = char >= search if l3 then min=search else max=search end search=min+((max-min)/2)/k10*k10",
            //    "l4 = char >= search if l4 then min=search else max=search end search=min+((max-min)/2)/k10*k10",
            //    ":out=search goto 1"
            //);

            //var ast = TestExecutor.Parse(
            //    "b*=2 flag=b>30 if flag then :b=a end if :a then b = 7 end goto 1",
            //    "goto 1"
            //);

            var ast = TestExecutor.Parse(
                "z = -1 z-- :a = z a = sin(:a * (z + 2)) a+=1 a /= z",
                "flag=a==:a if flag then goto 5 else goto 6 end b=0/0",
                ":x = \"hello\" * 4 goto \"world\" x = 2",
                "b*=2 flag=b>30 if flag then :b=a end if :a then b = 7 end",
                "b=b-1 goto 4",
                "b=b+1 goto 4"
            );

            //var ast = TestExecutor.Parse("d=r---r n=8-6*(d<5) n+=2*((d>n)-(d<n)) e+=(n+(d>n)-(d<n))*t^j++ goto 7");

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

            Console.WriteLine("## Input");
            Console.WriteLine(ast);
            Console.WriteLine();

            var hints = new[] {
                (new VariableName(":a"), Yolol.Execution.Type.Number)
            };

            var p = new OptimisationPipeline(2, false, hints);
            var r = await p.Apply(ast);
            Console.WriteLine("## Output");
            Console.WriteLine(r);

            //var cfg = new Builder(ast).Build();

            //cfg = cfg.Fixpoint(cf =>
            //{
            //    // Find types
            //    cf = cf.StaticSingleAssignment(out var ssa);
            //    cf = cf.FlowTypingAssignment(ssa, out var types, hints);

            //    // Optimise graph based on types
            //    cf = cf.VisitBlocks(() => new ConstantFoldingVisitor(true));
            //    cf = cf.VisitBlocks(t => new OpNumByConstNumCompressor(t), types);
            //    cf = cf.VisitBlocks(() => new ErrorCompressor());
            //    cf = cf.VisitBlocks(u => new RemoveUnreadAssignments(u, ssa), c => c.FindUnreadAssignments());
            //    cf = cf.FlowTypingAssignment(ssa, out types, hints);
            //    cf = cf.TypeDrivenEdgeTrimming(types);
            //    cf = cf.RemoveUnreachableBlocks();
            //    cf = cf.MergeAdjacentBasicBlocks();
            //    cf = cf.NormalizeErrors();

            //    // Optimise graph based on DFG
            //    cf = cf.FoldUnnecessaryCopies(ssa);

            //    // Remove SSA before refinding it in the next iteration
            //    cf = cf.RemoveStaticSingleAssignment(ssa);

            //    return cf;
            //});

            //cfg = cfg.Fixpoint(cf =>
            //{
            //    // Minify the graph
            //    cf = cf.ReplaceUnassignedReads();
            //    cf = cf.RemoveEmptyBlocks();
            //    cf = cf.RemoveUnreachableBlocks();

            //    return cf;
            //});

            //// Convert back into Yolol
            //Console.WriteLine("## Output");
            //var yolol = cfg.ToYolol().StripTypes();
            //Console.WriteLine(yolol);
            //Console.WriteLine();

            ////Console.WriteLine($"{ast.ToString().Length} => {yolol.ToString().Length}");
            ////Console.WriteLine();

            //Console.WriteLine("## CFG");
            //Console.WriteLine(cfg.ToDot());
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

            Console.WriteLine(ast);
            Console.WriteLine($"Score: {ast.ToString().Length}");
        }

        [TestMethod]
        public void ParsingNumbers()
        {
            for (var i = 0; i < 10; i++)
            {
                var result = TestExecutor.Execute(new ConstantNetwork(new KeyValuePair<string, Value>("input", i.ToString())),
                    "char = :input",
                    "min=0 max=10 search=5 k10=1000 k20=2*k10",
                    "l1 = char >= search min=l1--*search-l1*min max=-l1++*search+l1*max search=min+(max-min)/k20*k10",
                    "l2 = char >= search min=l2--*search-l2*min max=-l2++*search+l2*max search=min+(max-min)/k20*k10",
                    "l3 = char >= search min=l3--*search-l3*min max=-l3++*search+l3*max search=min+(max-min)/k20*k10",
                    "l4 = char >= search min=l4--*search-l4*min max=-l4++*search+l4*max search=min+(max-min)/k20*k10"
                );

                var l1 = result.GetVariable("l1").Value.Number;
                var l2 = result.GetVariable("l2").Value.Number;
                var l3 = result.GetVariable("l3").Value.Number;
                var l4 = result.GetVariable("l4").Value.Number;
                var s = result.GetVariable("search");

                Console.WriteLine($"i={i} {l4}{l3}{l2}{l1} {s}");

                Assert.AreEqual(i, s.Value.Number);
            }
        }

        //[TestMethod]
        //public void ParsingNumbers2()
        //{
        //    var patterns = new List<((int, int, int, int), int)>();

        //    for (var i = 0; i < 10; i++)
        //    {
        //        var result = TestExecutor.Execute(new ConstantNetwork(new KeyValuePair<string, Value>("input", i.ToString())),
        //            "i = :input",
        //            "a=i>7 b=(i>3)*(i<8) c=(i>1)*(i<4)+(i>5)*(i<8) goto a*4+b*2+c+3",
        //            ":output++",
        //            ":output++",
        //            ":output++",
        //            ":output++",
        //            ":output++",
        //            ":output++"
        //        );

        //        // a b c d
        //        // 0 0 0 0 => 0
        //        // 0 0 0 1 => 1
        //        // 0 0 1 0 => 2
        //        // 0 0 1 1 => 3
        //        // 0 1 0 0 => 4
        //        // 0 1 0 1 => 5
        //        // 0 1 1 0 => 6
        //        // 0 1 1 1 => 7
        //        // 1 0 0 0 => 8
        //        // 1 0 0 1 => 9

        //        var a = (int)result.GetVariable("a").Value.Number.Value;
        //        var b = (int)result.GetVariable("b").Value.Number.Value;
        //        var c = (int)result.GetVariable("c").Value.Number.Value;
        //        var d = (int)result.GetVariable("d").Value.Number.Value;

        //        patterns.Add(((a, b, c, d), i));
        //        //Console.WriteLine($"a:{a} b:{b} c:{c}  {i}");
        //    }

        //    var groups = patterns.GroupBy(a => a.Item1, a => a.Item2).ToArray();
        //    foreach (var grouping in groups.OrderBy(a => a.Key.Item1 * 4 + a.Key.Item2 * 2 + a.Key.Item3))
        //    {
        //        var (a, b, c, d) = grouping.Key;
        //        Console.WriteLine(a * 8 + b * 4 + c * 2 + d + " " + grouping.Key + " " + string.Join(" ", grouping));
        //    }

        //    Assert.Fail();
        //}

        //[TestMethod]
        //public void StringDecMagic()
        //{
        //    var result = TestExecutor.Execute(
        //        "x = \"42\"",
        //        "y = x - (--x)"
        //    );
        //    var x = result.GetVariable("x").Value.String;
        //    var y = result.GetVariable("y").Value.String;

        //    Console.WriteLine($"{x} {y}");

        //    Assert.AreEqual("", y);
        //}
    }
}

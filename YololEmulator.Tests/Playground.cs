using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Analysis;
using Yolol.Analysis.TreeVisitor;
using Yolol.Execution;
using Yolol.Grammar;
using Yolol.Grammar.AST.Statements;

namespace YololEmulator.Tests
{
    [TestClass]
    public class Playground
    {
        [TestMethod]
        public async Task Optimisation()
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

            //var ast = TestExecutor.Parse(
            //    "p = :p q = :q e = :e n = p * q phi = (p - 1) * (q - 1) t = 1",
            //    "a = e b = p - 1 d = 0 s = 2",
            //    "if (0 == a % 2 and 0 == b % 2) then a/= 2 b /= 2 d += 1 goto s+1 end",
            //    "if a != b then if 0 == a % 2 then a/= 2 goto s+2 end else goto s+4 end",
            //    "if 0 == b % 2 then b/= 2 else if a > b then a = (a - b) / 2 else b = (b - a) / 2 end end goto s+2",
            //    "if a != 1 then :answer = \"error: p=\" + p goto 6 end",
            //    "a = e b = q - 1 d = 0 s = 7",
            //    "if (0 == a % 2 and 0 == b % 2) then a/= 2 b /= 2 d += 1 goto s+1 end",
            //    "if a != b then if 0 == a % 2 then a/= 2 goto s+2 end else goto s+4 end",
            //    "if 0 == b % 2 then b/= 2 else if a > b then a = (a - b) / 2 else b = (b - a) / 2 end end goto s+2",
            //    "if a != 1 then :answer = \"error: q=\" + q goto 11 end",
            //    "a = 36 b = phi d = 0 s = 12",
            //    "if (0 == a % 2 and 0 == b % 2) then a/= 2 b /= 2 d += 1 goto s+1 end",
            //    "if a != b then if 0 == a % 2 then a/= 2 goto s+2 end else goto s+4 end",
            //    "if 0 == b % 2 then b/= 2 else if a > b then a = (a - b) / 2 else b = (b - a) / 2 end end goto s+2",
            //    "if a != 1 then :answer = \"error: phi=\" + phi goto 16 end",
            //    "if (1 % phi)== (t * e) % phi then goto 18 else t+=1 goto 17 end",  // changing t++ to t+=1 fixes the error (because of Phi nodes in Inc/Dec)
            //    ":pubkey_n = n :pubkey_e = e :privkey_n = n :privkey_t = t goto 18",
            //    "",
            //    "",
            //    ""  // Must add blank lines to keep quickfuzz happy
            //);

            // Fuzz fail because of yolol reconstruction adding additional blank lines. These lines are introduced because of `goto expr` type statements which _may_ jump to that line.
            var ast = TestExecutor.Parse(
                "z = -1 z-- :a = z a = sin(:a * (z + 2)) a+=1 a /= z",
                "flag=a==:a if not flag then goto 5 else goto 6 end b=0/0",
                ":x = \"hello\" * 4 goto \"world\" x = 2",
                "b*=2 flag=b>30 if flag then :b=a end if :a then b = 7 end",
                "b-=1 goto 4",
                "b+=1 goto 4"
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

            var p = new OptimisationPipeline(ast.Lines.Count, 2, false, hints);
            var r = await p.Apply(ast);
            Console.WriteLine("## Output");
            Console.WriteLine(r);
        }

        [TestMethod]
        public void ExprDecomposition()
        {
            var ast = TestExecutor.Parse(
                "a = b+c*-(d+z)%14+sin(3*2)+(y++)"
            );
            var ass = (Assignment)ast.Lines[0].Statements.Statements.Single();

            Console.WriteLine(ast);
            Console.WriteLine("---");

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
    }
}
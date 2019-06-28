using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Analysis.Reduction;

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
        public void MethodName()
        {
            var ast = TestExecutor.Parse("a=111111 b=111111 c=111111 d=222222 e=222222 f=3333 g=333 h=3333 i=\"hello\" j=\"hello\"");

            var reduced = ast.HoistConstants().SimplifyVariableNames();

            Console.WriteLine(reduced);
            //a=111111 b=222222 c="hello" d=a j=a m=a l=b e=b h=3333 g=333 k=3333 i=c f=c
        }

        [TestMethod]
        public void FinalGoto()
        {
            var ast = TestExecutor.Parse(
                "a=111111 b=111111 c=111111 goto 2",
                "d=222222 e=222222 f=3333 goto 4",
                "g=333 h=3333 i=\"hello\" j=\"hello\"");

            Console.WriteLine(ast);
            Console.WriteLine($"Score: {ast.ToString().Length}");
            Console.WriteLine();

            var reduced = ast.TrailingGotoNextLineElimination();

            Console.WriteLine(reduced);
            Console.WriteLine($"Score: {reduced.ToString().Length}");
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

        [TestMethod]
        public void StringIndexing()
        {
            var ast = TestExecutor.Parse(
                "s = \"ABCDEFGHIJKLMNOPQRSTUVWXYZ\"",
                "idx = 4",
                "s2 = s slen = 0",
                "s2-- slen++ if s2 != \"\" then goto 3 end",
                "s2 = s nback = slen - idx - 1",
                "if nback < 1 then goto 6 end s2-- nback-- goto 5",
                "char = (s2--) - s2",
                "goto 7"
            );

            Console.WriteLine(ast);
            Console.WriteLine($"Score: {ast.ToString().Length}");
            Console.WriteLine();
            
            ast = ast.FoldConstants();
            ast = ast.HoistConstants();
            ast = ast.SimplifyVariableNames();
            ast = ast.DeadPostGotoElimination();
            ast = ast.CompressCompoundIncrement();
            ast = ast.TrailingGotoNextLineElimination();
            ast = ast.TrailingConditionalGotoAnyLineCompression();
            ast = ast.CompressConditionalAssignment();

            Console.WriteLine(ast);
            Console.WriteLine($"Score: {ast.ToString().Length}");
        }
    }
}

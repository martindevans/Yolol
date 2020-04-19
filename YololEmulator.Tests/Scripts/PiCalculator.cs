using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Analysis.TreeVisitor.Reduction;

namespace YololEmulator.Tests.Scripts
{
    [TestClass]
    public class PiCalculator
    {
        [TestMethod]
        public void PiCalculatorMethod()
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
    }
}

//using System;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System.Collections.Generic;
//using System.Linq;
//using Yolol.Analysis.ControlFlowGraph.Extensions;
//using Yolol.Analysis.TreeVisitor.Reduction;
//using Yolol.Analysis.Types;
//using Yolol.Grammar;

//namespace YololEmulator.Tests.Analysis.Inspection
//{
//    [TestClass]
//    public class FindBooleansTests
//    {
//        private static void Test(IEnumerable<VariableName> bools, params string[] code)
//        {
//            var ast = TestExecutor.Parse(code);

//            var cfg = new Yolol.Analysis.ControlFlowGraph.Builder(ast.StripTypes(), code.Length).Build();
//            cfg = cfg.StaticSingleAssignment(out var ssa);
//            cfg = cfg.FlowTypingAssignment(ssa, out var types);

//            var variableNames = cfg.FindBooleanVariables(ssa, types).ToHashSet();

//            var ok = variableNames.IsSupersetOf(bools);
//            if (!ok)
//            {
//                Console.WriteLine("Not found: " + string.Join(",", bools.Where(b => !variableNames.Contains(b)).Select(a => a.ToString()).ToArray()));
//                Assert.Fail();
//            }
//        }

//        [TestMethod]
//        public void FindBooleansSingleLine()
//        {
//            var names = new VariableName[] {
//                new VariableName("a[0]"),
//                new VariableName("b[0]"),
//            };

//            Test(names, "a=30<2 theAnswer=42 b=theAnswer==42");
//        }

//        [TestMethod]
//        public void FindBooleanLiterals()
//        {
//            var names = new VariableName[] {
//                new VariableName("a[0]"),
//                new VariableName("b[0]")
//            };

//            Test(names, "a=1 theAnswer=42 b=theAnswer==a");
//        }

//        [TestMethod]
//        public void FindBooleansMultiLine()
//        {
//            var names = new VariableName[] {
//                new VariableName("a[0]"),
//                new VariableName("b[0]")
//            };

//            Test(names, "a=1", "theAnswer=42", "b=theAnswer==a");
//        }

//        [TestMethod]
//        public void FindBooleansMultiPathNotBothBoolean()
//        {
//            // There are two paths here:
//            //
//            //  - `a[0]` (line 1) is assigned a value and jumps to line 3
//            //  - `:a/1` (line 1) is an error (e.g. `:a` is a string) and goes to line 2, a[1] is assigned a bool
//            //
//            // In the first case `b` is a number, not a bool. In the second case b is a boolean.

//            var names = new VariableName[] {
//                new VariableName("a[1]"),
//            };

//            Test(names, "a=:a/1 goto 3", "a=1", "b=a");
//        }

//        [TestMethod]
//        public void FindBooleansMultiPathBothBoolean()
//        {
//            // There are two paths here:
//            //
//            //  - `a[0]` (line 1) is assigned a boolean value and jumps to line 3
//            //  - `:a/1` (line 1) is an error (e.g. `:a` is a string) and goes to line 2, a[1] is assigned a boolean value again
//            //
//            // In both cases `c` is a boolean

//            var names = new VariableName[] {
//                new VariableName("a[0]"),
//                new VariableName("a[1]"),
//                new VariableName("c[0]"),
//            };

//            Test(names, "a=1 b=:a/1 goto 3", "a=0", "c=a");
//        }

//        [TestMethod]
//        public void FindBooleansOpportunistic()
//        {
//            var names = new VariableName[] {
//                new VariableName("c[0]"),
//                new VariableName("d[0]")
//            };

//            Test(names, "a=0 b=1 c=a+b d=a*b e=b/a");
//        }
//    }
//}

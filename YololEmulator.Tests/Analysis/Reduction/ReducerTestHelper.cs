using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Grammar.AST;

namespace YololEmulator.Tests.Analysis.Reduction
{
    class ReducerTestHelper
    {
        private Func<Program, Program> reducer;

        public ReducerTestHelper(System.Func<Program, Program> reducer)
        {
            this.reducer = reducer;
        }

        public void Run(string inputYolol, string expectedReducedYolol) => Run(inputYolol, expectedReducedYolol, reducer);

        public void Run(Program inputAst, string expectedReducedYolol) => Run(inputAst, expectedReducedYolol, reducer);

        public void Run(string inputYolol, Program expectedReducedYolol) => Run(inputYolol, expectedReducedYolol, reducer);

        public void Run(Program inputAst, Program expectedReducedAst) => Run(inputAst, expectedReducedAst, reducer);

        public static void Run(string inputYolol, string expectedReducedYolol, Func<Program, Program> reducer)
        {
            var inputAst = TestExecutor.Parse(inputYolol);
            var expectedReducedAst = TestExecutor.Parse(expectedReducedYolol);

            Run(inputAst, expectedReducedAst, reducer);
        }

        public static void Run(Program inputAst, string expectedReducedYolol, Func<Program, Program> reducer)
        {
            var expectedReducedAst = TestExecutor.Parse(expectedReducedYolol);

            Run(inputAst, expectedReducedAst, reducer);
        }

        public static void Run(string inputYolol, Program expectedReducedAst, Func<Program, Program> reducer)
        {
            var inputAst = TestExecutor.Parse(inputYolol);

            Run(inputAst, expectedReducedAst, reducer);
        }

        public static void Run(Program inputAst, Program expectedReducedAst, Func<Program, Program> reducer)
        {
            var reducedAst = reducer(inputAst);

            Assert.IsTrue(expectedReducedAst.Equals(reducedAst));
        }
    }
}

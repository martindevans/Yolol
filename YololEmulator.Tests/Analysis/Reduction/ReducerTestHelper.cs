using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Grammar.AST;

namespace YololEmulator.Tests.Analysis.Reduction
{
    public class ReducerTestHelper
    {
        private readonly Func<Program, Program> _reducer;

        public ReducerTestHelper(Func<Program, Program> reducer)
        {
            this._reducer = reducer;
        }

        public void Run(string inputYolol, string expectedReducedYolol) => Run(inputYolol, expectedReducedYolol, _reducer);

        public void Run(Program inputAst, string expectedReducedYolol) => Run(inputAst, expectedReducedYolol, _reducer);

        public void Run(string inputYolol, Program expectedReducedYolol) => Run(inputYolol, expectedReducedYolol, _reducer);

        public void Run(Program inputAst, Program expectedReducedAst) => Run(inputAst, expectedReducedAst, _reducer);

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
            if (reducer == null)
                throw new ArgumentNullException(nameof(reducer));
            var reducedAst = reducer(inputAst);

            Assert.AreEqual(expectedReducedAst.ToString(), reducedAst.ToString());

            //todo: this test should use AST equality, but AST equality is arguably broken due to empty statement lists. Fix that and then enable this again.
            // Assert.IsTrue(expectedReducedAst.Equals(reducedAst));
        }
    }
}

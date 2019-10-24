using System;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Grammar.AST;

namespace YololEmulator.Tests.Analysis.Reduction
{
    public class ReducerTestHelper
    {
        private readonly Func<Program, Program> _reducer;

        public ReducerTestHelper([NotNull] Func<Program, Program> reducer)
        {
            this._reducer = reducer;
        }

        public void Run([NotNull] string inputYolol, [NotNull] string expectedReducedYolol) => Run(inputYolol, expectedReducedYolol, _reducer);

        public void Run([NotNull] Program inputAst, [NotNull] string expectedReducedYolol) => Run(inputAst, expectedReducedYolol, _reducer);

        public void Run([NotNull] string inputYolol, [NotNull] Program expectedReducedYolol) => Run(inputYolol, expectedReducedYolol, _reducer);

        public void Run([NotNull] Program inputAst, [NotNull] Program expectedReducedAst) => Run(inputAst, expectedReducedAst, _reducer);

        public static void Run([NotNull] string inputYolol, [NotNull] string expectedReducedYolol, [NotNull] Func<Program, Program> reducer)
        {
            var inputAst = TestExecutor.Parse(inputYolol);
            var expectedReducedAst = TestExecutor.Parse(expectedReducedYolol);

            Run(inputAst, expectedReducedAst, reducer);
        }
        public static void Run([NotNull] Program inputAst, [NotNull] string expectedReducedYolol, [NotNull] Func<Program, Program> reducer)
        {
            var expectedReducedAst = TestExecutor.Parse(expectedReducedYolol);

            Run(inputAst, expectedReducedAst, reducer);
        }
        
        public static void Run([NotNull] string inputYolol, [NotNull] Program expectedReducedAst, [NotNull] Func<Program, Program> reducer)
        {
            var inputAst = TestExecutor.Parse(inputYolol);

            Run(inputAst, expectedReducedAst, reducer);
        }

        public static void Run([NotNull] Program inputAst, [NotNull] Program expectedReducedAst, [NotNull] Func<Program, Program> reducer)
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

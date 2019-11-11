using Microsoft.Z3;
using System;

namespace Yolol.Analysis.SAT.Extensions
{
    public static class SolverExtensions
    {
        public static IDisposable UsePush(this Solver solver)
        {
            solver.Push();
            return new Pop(solver);
        }

        private class Pop
            : IDisposable
        {
            private readonly Solver _solver;

            public Pop(Solver solver)
            {
                _solver = solver;
            }

            public void Dispose()
            {
                _solver.Pop();
            }
        }

        public static bool IsSatisfiable(this Solver solver, BoolExpr expr)
        {
            using (solver.UsePush())
            {
                solver.Assert(expr);
                return solver.Check() == Status.SATISFIABLE;
            }
        }
    }
}

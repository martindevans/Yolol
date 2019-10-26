using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Yolol.Analysis.TreeVisitor.Inspection;
using Yolol.Grammar;
using Yolol.Grammar.AST.Expressions;

namespace Yolol.Analysis.ControlFlowGraph.Extensions
{
    public static class AnalysisExtensions
    {
        [NotNull] public static IReadOnlyDictionary<VariableName, BaseExpression> FindConstants([NotNull] this IControlFlowGraph cfg, ISingleStaticAssignmentTable ssa)
        {
            var constants = new Dictionary<VariableName, BaseExpression>();
            var count = -1;

            // Keep finding more constants until no more are found
            while (count != constants.Count)
            {
                count = constants.Count;
                cfg.VisitBlocks(() => new FindConstantVariables(constants, ssa));
            }

            return constants;
        }

        [NotNull] public static IReadOnlyCollection<VariableName> FindUnreadAssignments([NotNull] this IControlFlowGraph cfg)
        {
            var assigned = new FindAssignedVariables();
            var read = new FindReadVariables();

            foreach (var bb in cfg.Vertices)
            {
                assigned.Visit(bb);
                read.Visit(bb);
            }

            var result = new HashSet<VariableName>(assigned.Names.Where(n => !n.IsExternal));
            result.ExceptWith(read.Names);

            return result;
        }

        /// <summary>
        /// Get an enumerable of all variables which are read in the program with a count of how many times they are read
        /// </summary>
        /// <param name="cfg"></param>
        /// <returns></returns>
        [NotNull] public static IEnumerable<(VariableName, uint)> FindReadCounts([NotNull] this IControlFlowGraph cfg)
        {
            return from v in cfg.Vertices
                   from r in v.FindReadCounts()
                   group r.Item2 by r.Item1 into counts
                   let c = counts.Aggregate((a, b) => a + b)
                   select (counts.Key, c);
        }

        [NotNull] public static IEnumerable<(VariableName, uint)> FindReadCounts([NotNull] this IBasicBlock block)
        {
            var r = new FindReadVariables();
            r.Visit(block);

            return r.Counts;
        }

        [NotNull] public static IEnumerable<VariableName> FindWrites([NotNull] this IBasicBlock block, ISingleStaticAssignmentTable ssa)
        {
            var r = new FindAssignedVariables();
            r.Visit(block);

            return r.Names;
        }

        [NotNull]
        public static IReadOnlyCollection<VariableName> FindBooleanVariables([NotNull] this IControlFlowGraph cfg, ISingleStaticAssignmentTable ssa)
        {
            var booleans = new HashSet<VariableName>();

            var count = -1;

            // Keep finding more constants until no more are found
            while (count != booleans.Count)
            {
                count = booleans.Count;

                cfg.VisitBlocks(() => new FindBooleanVariables(booleans, ssa));
            }

            var result = new HashSet<VariableName>(booleans.Where(n => !n.IsExternal));

            return result;
        }
    }
}

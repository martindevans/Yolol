using System.Collections.Generic;
using System.Linq;
using Yolol.Analysis.TreeVisitor.Inspection;
using Yolol.Analysis.Types;
using Yolol.Grammar;
using Yolol.Grammar.AST.Expressions;

namespace Yolol.Analysis.ControlFlowGraph.Extensions
{
    public static class AnalysisExtensions
    {
        public static IReadOnlyDictionary<VariableName, BaseExpression> FindConstants(this IControlFlowGraph cfg, ISingleStaticAssignmentTable ssa)
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

        public static IReadOnlyCollection<VariableName> FindUnreadAssignments(this IControlFlowGraph cfg)
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
        public static IEnumerable<(VariableName, uint)> FindReadCounts(this IControlFlowGraph cfg)
        {
            return from v in cfg.Vertices
                   from r in v.FindReadCounts()
                   group r.Item2 by r.Item1 into counts
                   let c = counts.Aggregate((a, b) => a + b)
                   select (counts.Key, c);
        }

        public static IEnumerable<(VariableName, uint)> FindReadCounts(this IBasicBlock block)
        {
            var r = new FindReadVariables();
            r.Visit(block);

            return r.Counts.Select(a => (a.Key, a.Value));
        }

        public static IEnumerable<VariableName> FindWrites(this IBasicBlock block, ISingleStaticAssignmentTable ssa)
        {
            var r = new FindAssignedVariables();
            r.Visit(block);

            return r.Names;
        }

        /// <summary>
        /// Find variables which are guaranteed to be `0` or `1`
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="ssa"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static ISet<VariableName> FindBooleanVariables(this IControlFlowGraph cfg, ISingleStaticAssignmentTable ssa, ITypeAssignments types)
        {
            var booleans = new HashSet<VariableName>();

            // Keep finding more constants until no more are found
            var count = -1;
            while (count != booleans.Count)
            {
                count = booleans.Count;
                cfg.VisitBlocks(() => new FindBooleanVariables(booleans, ssa, types));
            }

            var result = new HashSet<VariableName>(booleans.Where(n => !n.IsExternal));

            return result;
        }
    }
}

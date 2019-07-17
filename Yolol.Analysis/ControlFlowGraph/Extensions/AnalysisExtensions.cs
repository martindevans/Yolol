using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Yolol.Analysis.TreeVisitor.Inspection;
using Yolol.Grammar;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.ControlFlowGraph.Extensions
{
    public static class AnalysisExtensions
    {
        [NotNull] public static IReadOnlyCollection<VariableName> FindUnreadAssignments([NotNull] this IControlFlowGraph cfg)
        {
            var assigned = new FindAssignedVariables();
            var read = new FindReadVariables();

            foreach (var bb in cfg.Vertices)
            {
                var p = new Program(new[] {new Line(new StatementList(bb.Statements))});
                assigned.Visit(p);
                read.Visit(p);
            }

            var result = new HashSet<VariableName>(assigned.Names.Where(n => !n.IsExternal));
            result.ExceptWith(read.Names);

            return result;
        }
    }
}

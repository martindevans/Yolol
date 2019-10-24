using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.DataFlowGraph.Extensions
{
    public static class DotFormatExtensions
    {
        [NotNull] public static IEnumerable<BaseStatement> ToYolol([NotNull] this IDataFlowGraph dfg)
        {
            foreach (var dfgo in dfg.Outputs)
                yield return dfgo.ToStatement();
        }

        [NotNull] public static string ToDot([NotNull] this IDataFlowGraph dfg)
        {
            string GetNodeId(IDataFlowGraphNode node) => $"_{node.Id.ToString().Replace("-", "")}";

            var sb = new StringBuilder();
            sb.AppendLine("digraph {");
            sb.AppendLine("  node [fontsize = \"12\"];");
            sb.AppendLine("  edge [fontsize = \"8\"];");

            // Add clusters for input/output variables
            void OutputNameSection<T>(IEnumerable<T> nodes, string rank, string cluster, Func<T, string> label, Func<T, bool> external)
                where T : IDataFlowGraphNode
            {
                foreach (var item in nodes)
                    sb.AppendLine($"  {GetNodeId(item)} [rank={rank} label=\"{label(item)}\" shape={(external(item) ? "diamond" : "circle")}];");

                sb.AppendLine($"    {{rank=same { string.Join(" ", nodes.Select(a => GetNodeId(a))) }}}");
                if (nodes.Count() > 1)
                    sb.AppendLine($"    { string.Join(" -> ", nodes.Select(a => GetNodeId(a))) } [style=invis]");
            }
            OutputNameSection(dfg.Inputs, "min", "inputs", a => a.ToString(), a => (a as IDataFlowGraphInputVariable)?.Name.IsExternal ?? false);
            OutputNameSection(dfg.Outputs, "max", "outputs", a => a.ToString(), a => false);

            // Insert an invisible edge to line up the input and output blocks
            if (dfg.Inputs.Any() && dfg.Outputs.Any())
                sb.AppendLine($"  {GetNodeId(dfg.Inputs.First())} -> {GetNodeId(dfg.Outputs.First())} [style=invis]");

            // Emit a node for every intermediate op
            foreach (var op in dfg.Operations)
                sb.AppendLine($"  {GetNodeId(op)} [label=\"{op}\", shape=square];");

            // Emit connections for nodes
            foreach (var output in dfg.Outputs)
                sb.Append($"  {GetNodeId(output.Input)} -> {GetNodeId(output)}");
            foreach (var op in dfg.Operations)
                foreach (var input in op.Inputs)
                    sb.AppendLine($"  {GetNodeId(input)} -> {GetNodeId(op)}");

            sb.AppendLine();
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}

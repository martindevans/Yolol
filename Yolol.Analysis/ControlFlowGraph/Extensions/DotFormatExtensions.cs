using System;
using System.Linq;
using System.Text;

namespace Yolol.Analysis.ControlFlowGraph.Extensions
{
    /// <summary>
    /// Represents the default DOT formatter.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class DotFormatExtensions
    {
        private static string ID(Guid guid)
        {
            return $"_{guid.ToString().Replace("-", "_")}";
        }

        public static string ToDot(this IControlFlowGraph graph)
        {
            var sb = new StringBuilder();

            sb.AppendLine("digraph {");
            sb.AppendLine("  node [fontsize = \"12\"];");
            sb.AppendLine("  edge [fontsize = \"8\"];");

            // Add root node
            var root = graph.Vertices.Single(a => a.Type == BasicBlockType.Entry);
            sb.AppendLine($"  {ID(root.ID)} [label=\"entry\" shape=circle rank=min];");

            static string EdgeAsString(IEdge edge)
            {
                // 35 -> 36 [label="eol_fallthrough" color="r g b"];
                var a = ID(edge.Start.ID);
                var b = ID(edge.End.ID);

                var col = "#000000";
                if (edge.Type == EdgeType.GotoConstStr || edge.Type == EdgeType.RuntimeError)
                    col = "#ff0000";

                return $"  {a} -> {b} [label=\"{edge.Type}\" color=\"{col}\"];";
            }

            // Group edges into clusters (transfers within one line)
            var clusters = from edge in graph.Edges
                           where edge.Start.LineNumber == edge.End.LineNumber
                           group edge by edge.Start.LineNumber into grps
                           select grps;
            var others = from edge in graph.Edges
                         where edge.Start.LineNumber != edge.End.LineNumber
                         select edge;

            foreach (var cluster in clusters)
            {
                sb.AppendLine($"  subgraph cluster_L{cluster.Key} {{");
                foreach (var edge in cluster)
                    sb.AppendLine($"    {EdgeAsString(edge)}");
                sb.AppendLine("  }");
            }

            foreach (var edge in others)
                sb.AppendLine($"{EdgeAsString(edge)}");

            var vs = graph.Vertices.Where(a => a.Type != BasicBlockType.Entry).Select(vertex => {
                var style = vertex.Type == BasicBlockType.LineStart ? "style=none" : "style=rounded";
                return $"  {ID(vertex.ID)} [label=\"{vertex.ToString()!.Replace("\"", "'")}\" shape=box {style}]";
            });

            sb.AppendJoin(";\n", vs);
            sb.AppendLine();
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}

using System;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.ControlFlowGraph.Extensions
{
    /// <summary>
    /// Represents the default DOT formatter.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class DotFormatExtensions
    {
        [NotNull]
        private static string ID(Guid guid)
        {
            return $"_{guid.ToString().Replace("-", "_")}";
        }

        public static string ToDot([NotNull] this IControlFlowGraph graph)
        {
            var sb = new StringBuilder();

            sb.AppendLine("digraph {");
            sb.AppendLine("  node [fontsize = \"12\"];");
            sb.AppendLine("  edge [fontsize = \"8\"];");

            // Add root node
            var root = graph.Vertices.Single(a => a.Type == BasicBlockType.Entry);
            sb.AppendLine($"  {ID(root.ID)} [label=\"entry\" shape=note rank=min];");

            foreach (var edge in graph.Edges)
            {
                // 35 -> 36 [label="eol_fallthrough" color="r g b"];
                var a = ID(edge.Start.ID);
                var b = ID(edge.End.ID);

                string col;
                switch (edge.Type)
                {
                    case EdgeType.GotoConstStr:
                    case EdgeType.RuntimeError:
                        col = "#ff0000";
                        break;
                    default:
                        col = "#000000";
                        break;
                }

                sb.AppendLine($"  {a} -> {b} [label=\"{edge.Type}\" color=\"{col}\"];");
            }

            var vs = graph.Vertices.Where(a => a.Type != BasicBlockType.Entry).Select(vertex => {
                var style = vertex.Type == BasicBlockType.LineStart ? "style=none" : "style=rounded";
                return $"  {ID(vertex.ID)} [label=\"{vertex.ToString().Replace("\"", "'")}\" shape=box {style}]";
            });

            sb.AppendJoin(";\n", vs);
            sb.AppendLine();
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}

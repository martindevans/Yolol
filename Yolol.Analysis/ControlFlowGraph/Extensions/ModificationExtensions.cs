using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Yolol.Analysis.ControlFlowGraph.Extensions
{
    public static class ModificationExtensions
    {
        [NotNull] private static IReadOnlyDictionary<IBasicBlock, IMutableBasicBlock> CloneVertices(
            [NotNull] this IControlFlowGraph input,
            ControlFlowGraph output,
            [NotNull] Func<IBasicBlock, bool> keep,
            [NotNull] Action<IBasicBlock, IMutableBasicBlock> copy = null
            )
        {
            // Clone vertices (without edges)
            var replacements = new Dictionary<IBasicBlock, IMutableBasicBlock>();
            foreach (var vertex in input.Vertices.Where(keep))
            {
                var r = output.CreateNewBlock(vertex.Type, vertex.LineNumber, vertex.ID);
                replacements.Add(vertex, r);

                if (copy != null)
                    copy(vertex, r);
                else
                    foreach (var stmt in vertex.Statements)
                        r.Add(stmt);
            }

            return replacements;
        }

        [NotNull]
        private static IReadOnlyDictionary<IEdge, IEdge> CloneEdges(
            [NotNull] this IControlFlowGraph input,
            ControlFlowGraph output,
            IReadOnlyDictionary<IBasicBlock, IMutableBasicBlock> vertexReplacements,
            [NotNull] Func<IEdge, bool> keep)
        {
            var replacements = new Dictionary<IEdge, IEdge>();

            foreach (var edge in input.Edges)
            {
                if (!keep(edge))
                    continue;

                replacements.Add(
                    edge,
                    output.CreateEdge(vertexReplacements[edge.Start], vertexReplacements[edge.End], edge.Type)
                );
            }

            return replacements;
        }


        /// <summary>
        /// Remove vertices from the graph
        /// </summary>
        /// <param name="input"></param>
        /// <param name="keep"></param>
        /// <returns></returns>
        [NotNull] public static IControlFlowGraph Trim([NotNull] this IControlFlowGraph input, [NotNull] Func<IBasicBlock, bool> keep)
        {
            var cfg = new ControlFlowGraph();

            var rv = input.CloneVertices(cfg, keep);
            var _ = input.CloneEdges(cfg, rv, a => rv.ContainsKey(a.Start) && rv.ContainsKey(a.End));

            return cfg;
        }

        /// <summary>
        /// Remove edges from the graph
        /// </summary>
        /// <param name="input"></param>
        /// <param name="keep"></param>
        /// <returns></returns>
        [NotNull] public static IControlFlowGraph Trim([NotNull] this IControlFlowGraph input, [NotNull] Func<IEdge, bool> keep)
        {
            var cfg = new ControlFlowGraph();

            var replacementVertices = input.CloneVertices(cfg, __ => true);
            var _ = input.CloneEdges(cfg, replacementVertices, keep);

            return cfg;
        }


        /// <summary>
        /// Copy graph and modify vertices
        /// </summary>
        /// <param name="input"></param>
        /// <param name="copy"></param>
        /// <returns></returns>
        [NotNull] public static IControlFlowGraph Modify([NotNull] this IControlFlowGraph input, [NotNull] Action<IBasicBlock, IMutableBasicBlock> copy)
        {
            var cfg = new ControlFlowGraph();

            // Copy vertices (but leave empty)
            var replacementVertices = input.CloneVertices(cfg, __ => true, (___, ____) => { });

            // Copy edges
            var _ = input.CloneEdges(cfg, replacementVertices, __ => true);

            // Apply clone function to vertices
            foreach (var (key, value) in replacementVertices)
                copy(key, value);

            return cfg;
        }


        /// <summary>
        /// Add edges to the graph
        /// </summary>
        /// <param name="input"></param>
        /// <param name="create"></param>
        /// <returns></returns>
        [NotNull] public static IControlFlowGraph Add([NotNull] this IControlFlowGraph input, [NotNull] IEnumerable<(Guid, Guid, EdgeType)> create)
        {
            var cfg = new ControlFlowGraph();

            // Copy across all vertices and edges
            var rv = input.CloneVertices(cfg, __ => true);
            var _ = input.CloneEdges(cfg, rv, __ => true);

            // Add the extra edges
            foreach (var (start, end, type) in create)
            {
                var a = rv[input.Vertex(start)];
                var b = rv[input.Vertex(end)];
                cfg.CreateEdge(a, b, type);
            }

            return cfg;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Yolol.Analysis.ControlFlowGraph.Extensions
{
    public static class ReductionExtensions
    {
        /// <summary>
        /// Remove blocks that cannot be reached from the entry node
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        [NotNull] public static IControlFlowGraph RemoveUnreachableBlocks([NotNull] this IControlFlowGraph graph)
        {
            var reachable = new HashSet<IBasicBlock>();
            var queue = new Queue<IBasicBlock>();

            void Mark(IBasicBlock block)
            {
                if (reachable.Add(block))
                    foreach (var edge in block.Outgoing)
                        queue.Enqueue(edge.End);
            }

            queue.Enqueue(graph.Vertices.Single(v => v.Type == BasicBlockType.Entry));
            while (queue.Count > 0)
                Mark(queue.Dequeue());

            return graph.Trim(reachable.Contains);
        }

        /// <summary>
        /// Bypass nodes which contain no statements
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        [NotNull] public static IControlFlowGraph RemoveEmptyNodes([NotNull] this IControlFlowGraph graph)
        {
            // Find all the vertices we can shortcut
            var emptyVertices = new HashSet<IBasicBlock>(
                from vertex in graph.Vertices
                where vertex.Type == BasicBlockType.Basic
                where !vertex.Statements.Any()
                where vertex.Outgoing.Count() == 1
                where vertex.Outgoing.Single().Type == EdgeType.Continue
                select vertex
            );

            // Select all edges to delete
            var edgesDelete = new HashSet<IEdge>(
                from vertex in emptyVertices
                from input in vertex.Incoming
                select input
            );

            // Select edges to create
            var edgesCreate = new HashSet<(Guid, Guid, EdgeType)>(
                from vertex in emptyVertices
                from input in vertex.Incoming
                let output = vertex.Outgoing.Single()
                select (input.Start.ID, output.End.ID, input.Type)
            );

            var g = graph;
            g = g.Trim(e => !edgesDelete.Contains(e));
            g = g.Add(edgesCreate);
            return g;
        }
    }
}

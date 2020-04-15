using System;
using System.Collections.Generic;
using System.Linq;
using Yolol.Analysis.TreeVisitor;
using Yolol.Grammar.AST;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.ControlFlowGraph.Extensions
{
    public static class ModificationExtensions
    {
        private static IReadOnlyDictionary<IBasicBlock, IMutableBasicBlock> CloneVertices(
            this IControlFlowGraph input,
            IMutableControlFlowGraph output,
            Func<IBasicBlock, bool> keep,
            Action<IBasicBlock, IMutableBasicBlock>? copy = null
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

        private static IReadOnlyDictionary<IEdge, IEdge> CloneEdges(
            this IControlFlowGraph input,
            IMutableControlFlowGraph output,
            IReadOnlyDictionary<IBasicBlock, IMutableBasicBlock> vertexReplacements,
            Func<IEdge, bool> keep)
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
        public static IMutableControlFlowGraph Trim(this IControlFlowGraph input, Func<IBasicBlock, bool> keep)
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
        public static IMutableControlFlowGraph Trim(this IControlFlowGraph input, Func<IEdge, bool> keep)
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
        public static IMutableControlFlowGraph Modify(this IControlFlowGraph input, Action<IBasicBlock, IMutableBasicBlock> copy)
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

        public static IMutableControlFlowGraph Modify(this IControlFlowGraph input, Action<IEdge, Action<IBasicBlock, IBasicBlock, EdgeType>> copy)
        {
            var cfg = new ControlFlowGraph();

            // Copy vertices
            var replacementVertices = input.CloneVertices(cfg, __ => true);

            // Copy edges
            foreach (var edge in input.Edges)
            {
                copy(edge, (s, e, t) => {
                    var ss = replacementVertices[s];
                    var ee = replacementVertices[e];
                    cfg.CreateEdge(ss, ee, t);
                });
            }

            return cfg;
        }

        /// <summary>
        /// Add edges to the graph
        /// </summary>
        /// <param name="input"></param>
        /// <param name="create"></param>
        /// <returns></returns>
        public static IMutableControlFlowGraph Add(this IControlFlowGraph input, IEnumerable<(Guid, Guid, EdgeType)> create)
        {
            var cfg = new ControlFlowGraph();

            // Copy across all vertices and edges
            var rv = input.CloneVertices(cfg, __ => true);
            var _ = input.CloneEdges(cfg, rv, __ => true);

            // Add the extra edges
            foreach (var (start, end, type) in create)
            {
                var a = rv[input.Vertex(start)!];
                var b = rv[input.Vertex(end)!];
                cfg.CreateEdge(a, b, type);
            }

            return cfg;
        }

        /// <summary>
        /// Apply an AST visitor to each control flow graph vertex
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static IMutableControlFlowGraph VisitBlocks<T>(this IControlFlowGraph input, Func<T> factory)
            where T : BaseTreeVisitor
        {
            return input.Modify((a, b) => {

                // Create a program from the statements in this block, visit it
                var visited = factory().Visit(new Program(new[] { new Line(new StatementList(a.Statements)) }));

                // Extract statements from single line of result programand copy into result block
                foreach (var stmt in  visited.Lines.Single().Statements.Statements)
                    b.Add(stmt);
            });
        }

        /// <summary>
        /// Apply an AST visitor to each control flow graph vertex
        /// </summary>
        /// <typeparam name="TVisitor"></typeparam>
        /// <typeparam name="TPrep"></typeparam>
        /// <param name="input"></param>
        /// <param name="factory"></param>
        /// <param name="prepare"></param>
        /// <returns></returns>
        public static IMutableControlFlowGraph VisitBlocks<TVisitor, TPrep>(this IControlFlowGraph input, Func<TPrep, TVisitor> factory, Func<IControlFlowGraph, TPrep> prepare)
            where TVisitor : BaseTreeVisitor
        {
            var prep = prepare(input);
            return VisitBlocks(input, () => factory(prep));
        }

        public static IMutableControlFlowGraph VisitBlocks<TVisitor, TContext>(this IControlFlowGraph input, Func<TContext, TVisitor> factory, TContext context)
            where TVisitor : BaseTreeVisitor
        {
            return VisitBlocks(input, () => factory(context));
        }
    }
}

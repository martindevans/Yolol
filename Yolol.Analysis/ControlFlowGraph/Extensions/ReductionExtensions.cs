using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Yolol.Analysis.ControlFlowGraph.AST;
using Yolol.Analysis.DataFlowGraph;
using Yolol.Analysis.TreeVisitor;
using Yolol.Analysis.TreeVisitor.Inspection;
using Yolol.Analysis.TreeVisitor.Modification;
using Yolol.Analysis.TreeVisitor.Reduction;
using Yolol.Analysis.Types;
using Yolol.Grammar;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Expressions.Binary;
using Yolol.Grammar.AST.Expressions.Unary;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.ControlFlowGraph.Extensions
{
    public static class ReductionExtensions
    {
        [NotNull] public static IControlFlowGraph RecomposeModify([NotNull] this IControlFlowGraph cfg)
        {
            return cfg.VisitBlocks(() => new RecomposeModifyIR());
        }

        private class RecomposeModifyIR
            : BaseTreeVisitor
        {
            protected override BaseStatement Visit(Assignment ass)
            {
                if (ass.Right is Increment inc)
                {
                    if (!ass.Left.Equals(inc.Name))
                        throw new InvalidOperationException("Modify expression not in form of `a=inc(a)`");

                    return new ExpressionWrapper(new PostIncrement(ass.Left));
                }
                else if (ass.Right is Decrement dec)
                {
                    if (!ass.Left.Equals(dec.Name))
                        throw new InvalidOperationException("Modify expression not in form of `a=dec(a)`");

                    return new ExpressionWrapper(new PostDecrement(ass.Left));
                }
                else
                    return base.Visit(ass);
            }

            protected override BaseStatement Visit(TypedAssignment ass)
            {
                return Visit((Assignment)ass);
            }
        }

        /// <summary>
        /// Replace expressions which result in a constant with the result
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="ssa"></param>
        /// <returns></returns>
        [NotNull] public static IControlFlowGraph FoldConstants([NotNull] this IControlFlowGraph cfg, ISingleStaticAssignmentTable ssa)
        {
            // Keep finding and replacing constants until nothing is found
            cfg = cfg.Fixpoint(c =>
            {

                // Find variables which are assigned a value which is not tainted by external reads
                var constants = c.FindConstants(ssa);

                // Replace reads of a constant variable with the expression assigned to that variable
                c = c.VisitBlocks(() => new ReplaceConstantSubexpressions(constants));

                return c;
            });

            // Replace constant subexpressions with their value
            cfg = cfg.VisitBlocks(() => new ConstantFoldingVisitor(true));

            return cfg;
        }

        /// <summary>
        /// Replace inc/dec operations on numbers with a+=1 and a-=1
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        [NotNull] public static IControlFlowGraph SimplifyModificationExpressions([NotNull] this IControlFlowGraph cfg, [NotNull] ITypeAssignments types)
        {
            return cfg.VisitBlocks(() => new SimplifyModify(types));
        }

        private class SimplifyModify
            : BaseTreeVisitor
        {
            private readonly ITypeAssignments _types;

            public SimplifyModify(ITypeAssignments types)
            {
                _types = types;
            }

            protected override BaseExpression Visit(Decrement dec)
            {
                var t = _types.TypeOf(dec.Name);

                if (t != Execution.Type.Number)
                    return base.Visit(dec);
                else 
                    return new Subtract(new Variable(dec.Name), new ConstantNumber(1));
            }

            protected override BaseExpression Visit(Increment inc)
            {
                var t = _types.TypeOf(inc.Name);

                if (t != Execution.Type.Number)
                    return base.Visit(inc);
                else 
                    return new Add(new Variable(inc.Name), new ConstantNumber(1));
            }
        }

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
        [NotNull] public static IControlFlowGraph RemoveEmptyBlocks([NotNull] this IControlFlowGraph graph)
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

        /// <summary>
        /// Removed error/continue edges which we know cannot happen due to type info
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        [NotNull] public static IControlFlowGraph TypeDrivenEdgeTrimming([NotNull] this IControlFlowGraph graph, ITypeAssignments types)
        {
            return graph.Trim(edge => {

                // Find last statement in previous block
                var stmt = edge.Start.Statements.LastOrDefault();
                var tass = stmt as TypedAssignment;
                var err = stmt as ErrorStatement;

                // If type is unassigned we can't make a judgement
                if (tass?.Type == Execution.Type.Unassigned)
                    return true;

                if (edge.Type == EdgeType.RuntimeError)
                {
                    // If it's an error statement keep it
                    if (err != null)
                        return true;

                    // If there is no statement at all then it can't be an error
                    if (tass == null)
                        return false;

                    // Only keep edge if type has an error
                    return tass.Type.HasFlag(Execution.Type.Error);
                }
                else
                {
                    // If it's an error statement remove it
                    if (err != null)
                        return false;

                    // If there is no typed assignment we can't judge
                    if (tass == null)
                        return true;

                    return tass.Type != Execution.Type.Error;
                }
            });
        }

        /// <summary>
        /// Replaces nodes with an error statement and an error edge with an empty node and a continue edge
        /// </summary>
        /// <param name="cfg"></param>
        /// <returns></returns>
        [NotNull] public static IControlFlowGraph NormalizeErrors([NotNull] this IControlFlowGraph cfg)
        {
            var todo = new HashSet<IBasicBlock>();

            // Remove the error statements and save blocks to remove edges from
            cfg = cfg.Modify((a, b) => {

                if (!a.Statements.Any())
                    return;

                var toCopy = a.Statements;
                if ((a.Statements.Last() is ErrorStatement) && a.Outgoing.Count() == 1 && a.Outgoing.Single().Type == EdgeType.RuntimeError)
                {
                    toCopy = a.Statements.Take(a.Statements.Count() - 1);
                    todo.Add(b);
                }

                foreach (var stmt in toCopy)
                    b.Add(stmt);
            });

            // Copy graph, replacing edges as necessary
            return cfg.Modify((e, c) => {
                c(e.Start, e.End, todo.Contains(e.Start) ? EdgeType.Continue : e.Type);
            });
        }

        /// <summary>
        /// Merge together basic blocks on the same line connected only by a continue edge
        /// </summary>
        /// <param name="cfg"></param>
        /// <returns></returns>
        [NotNull] public static IControlFlowGraph MergeAdjacentBasicBlocks([NotNull] this IControlFlowGraph cfg)
        {
            // This keeps looping until it finds no work to do
            while (true)
            {
                // Find all candidates for merging
                var candidates = (from vertex in cfg.Vertices
                                  where vertex.Type == BasicBlockType.Basic
                                  where vertex.Outgoing.Count() == 1
                                  let outgoing = vertex.Outgoing.Single()
                                  where outgoing.Type == EdgeType.Continue
                                  where outgoing.End.LineNumber == vertex.LineNumber
                                  where outgoing.End.Type == BasicBlockType.Basic
                                  where outgoing.End.Incoming.Count() == 1
                                  select (vertex, outgoing.End));

                // Select a single candidate pair (A -> B), if there is no work then exit now
                var work = candidates.FirstOrDefault();
                if (work == default)
                    return cfg;

                // Move all the items from the A into B, leaving A empty
                cfg = cfg.Modify((a, b) => {
                    if (a.ID == work.End.ID)
                    {
                        work.vertex.CopyTo(b);
                        a.CopyTo(b);
                    }
                    else if (a.ID != work.vertex.ID)
                        a.CopyTo(b);
                });

                // Remove all empty blocks
                cfg = cfg.RemoveEmptyBlocks();
            }
        }

        /// <summary>
        /// Replace reads from variables which are never assigned with a constant `0` (the default value for unassigned vars)
        /// </summary>
        /// <param name="cfg"></param>
        /// <returns></returns>
        [NotNull] public static IControlFlowGraph ReplaceUnassignedReads([NotNull] this IControlFlowGraph cfg)
        {
            var v = new FindAssignedVariables();
            foreach (var vertex in cfg.Vertices)
                v.Visit(vertex);

            return cfg.VisitBlocks(() => new ReplaceUnassignedReads(new HashSet<VariableName>(v.Names)));
        }

        /// <summary>
        /// Replace a copy along a chain of variables (e.g. `b = a; c = b`) with a direct copy (e.g. `b = a; c = a`)
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="ssa"></param>
        /// <returns></returns>
        [NotNull] public static IControlFlowGraph FoldUnnecessaryCopies([NotNull] this IControlFlowGraph cfg, [NotNull] ISingleStaticAssignmentTable ssa)
        {
            IControlFlowGraph InnerFold(IControlFlowGraph cfgi)
            {
                // Copy across vertices, modifying them in the process
                return cfgi.Modify((a, b) =>
                {
                    // generate data flow graph for this block
                    var dfg = (IDataFlowGraph)new DataFlowGraph.DataFlowGraph(a, ssa);

                    // Count how many times each variable in the whole program is read
                    var readsInProgram = cfgi.FindReadCounts().ToDictionary(x => x.Item1, x => x.Item2);

                    // Find variables which are written in this block (we're in SSA, so everything is written exactly once)
                    var writesInBlock = a.FindWrites(ssa).ToHashSet();

                    // Find variables which are written in this block and read (just once) in this block
                    var copiesInBlock = (
                        from x in a.FindReadCounts()
                        where writesInBlock.Contains(x.Item1)
                        where x.Item2 == readsInProgram[x.Item1]
                        where x.Item2 == 1
                        where !x.Item1.IsExternal
                        select x.Item1
                    ).ToArray();

                    // Select a single var to optimise
                    var work = (
                        from op in dfg.Outputs
                        let ass = op.ToStatement() as Assignment
                        where ass != null
                        where copiesInBlock.Contains(ass.Left)
                        where ass.Right is Variable
                        select (ass, op)
                    ).FirstOrDefault();

                    // Early out if there is nothing to optimise 
                    if (work == default)
                    {
                        a.CopyTo(b);
                        return;
                    }

                    // Select the variable to replace and the expression to replace it with
                    var varToReplace = work.ass.Left;
                    var expToSubstitute = work.ass.Right;

                    var modified = new SubstituteVariable(varToReplace, expToSubstitute).Visit(a);
                    foreach (var item in modified.Statements)
                        b.Add(item);
                });
            }

            // Keep applying folding until no more folds are done
            return cfg.Fixpoint(InnerFold);
        }

    }
}

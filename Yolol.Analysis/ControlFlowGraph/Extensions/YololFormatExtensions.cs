using System;
using System.Collections.Generic;
using System.Linq;
using Yolol.Analysis.ControlFlowGraph.AST;
using Yolol.Grammar.AST;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.ControlFlowGraph.Extensions
{
    public static class YololFormatExtensions
    {
        public static Program ToYolol(this IControlFlowGraph cfg)
        {
            // Convert all lines with code on them
            var lines = from start in cfg.Vertices.Where(v => v.Type == BasicBlockType.LineStart)
                        let line = ConvertLine(start)
                        let index = start.LineNumber
                        orderby index
                        select (index, line);

            // Insert blank lines between converted lines as necessary
            var output = new List<Line>();
            var next = 1;
            foreach (var (index, line) in lines)
            {
                while (next != index)
                {
                    output.Add(new Line(new StatementList()));
                    next++;
                }

                output.Add(line);
                next++;
            }

            return new Program(output);
        }

        private static Line ConvertLine(IBasicBlock entry)
        {
            var statements = new List<BaseStatement>();

            var e = entry.Outgoing.Single();
            if (e.Type != EdgeType.Continue)
                throw new InvalidOperationException("LineStart block has an invalid edge");

            if (e.End.LineNumber == e.Start.LineNumber)
                RecursiveConvertBlock(e.End, statements);

            return new Line(new StatementList(statements));
        }

        /// <summary>
        /// Convert a basic block, recursively convert linked blocks on the same line which are not the stop block
        /// </summary>
        /// <param name="block"></param>
        /// <param name="output"></param>
        /// <param name="stop"></param>
        private static void RecursiveConvertBlock(IBasicBlock block, List<BaseStatement> output, IBasicBlock? stop = null)
        {
            if (stop != null && stop.ID.Equals(block.ID))
                return;

            // Copy statements from this block into output
            output.AddRange(block.Statements);

            // if a block is empty there's nothing more to do
            if (!block.Statements.Any())
                return;

            // If we've jumped away there's nothing more to convert on this line
            if (block.Statements.Last() is Goto)
                return;

            // Serialize if statement
            if (block.Statements.Last() is Conditional con)
            {
                // Remove `Conditional` node, we'll replace with an `if`
                output.RemoveAt(output.Count - 1);

                // Find conditional edges leaving this block
                var trueEdge = block.Outgoing.Single(a => a.Type == EdgeType.ConditionalTrue);
                var falseEdge = block.Outgoing.Single(a => a.Type == EdgeType.ConditionalFalse);

                // There are four possible cases:
                // - True/False branches both eventually pass on control flow to a common block (continueWith is common block)
                // - True jumps away, False is effectively the rest of the line (continueWith is false branch)
                // - False jumps away, True is effectively the rest of the line (continueWith is true branch)
                // - Both jump away, this is the end of the line (continueWith is null)

                // Try to find the first common block on this line
                var common = FindCommonSubsequentBlock(trueEdge.End, falseEdge.End);

                if (common != null)
                {
                    // Serialize both branches up to common block
                    var trueStmts = new List<BaseStatement>();
                    RecursiveConvertBlock(trueEdge.End, trueStmts, common);

                    var falseStmts = new List<BaseStatement>();
                    RecursiveConvertBlock(falseEdge.End, falseStmts, common);

                    output.Add(new If(con.Condition, new StatementList(trueStmts), new StatementList(falseStmts)));
                }
                else
                {
                    var trueStmts = new List<BaseStatement>();
                    RecursiveConvertBlock(trueEdge.End, trueStmts);

                    var falseStmts = new List<BaseStatement>();
                    RecursiveConvertBlock(falseEdge.End, falseStmts);

                    output.Add(new If(con.Condition, new StatementList(trueStmts), new StatementList(falseStmts)));
                    return;
                }

                // Continue on with rest of the line
                RecursiveConvertBlock(common, output, stop);
            }

            // Try to continue to the next block
            var e = block.Outgoing.SingleOrDefault(a => a.Type == EdgeType.Continue);
            if (e != null && e.Type == EdgeType.Continue && e.End.LineNumber == block.LineNumber && e.End != stop)
                RecursiveConvertBlock(e.End, output, stop);
        }

        private static IBasicBlock? FindCommonSubsequentBlock(IBasicBlock a, IBasicBlock b)
        {
            static IReadOnlyDictionary<IBasicBlock, uint> Visit(IBasicBlock block, uint depth, Dictionary<IBasicBlock, uint> depths)
            {
                // If we've already been to this block by a shorter route early exit
                if (depths.TryGetValue(block, out var prevDepth))
                    if (prevDepth < depth)
                        return depths;

                // Discovered this block
                depths[block] = depth;

                // Get edges we care about
                var outs = block.Outgoing.Where(e => (e.Type == EdgeType.Continue || e.Type == EdgeType.ConditionalTrue || e.Type == EdgeType.ConditionalFalse) && e.Start.LineNumber == e.End.LineNumber);

                foreach (var @out in outs)
                    Visit(@out.End, depth + 1, depths);

                return depths;
            }

            var ad = Visit(a, 0, new Dictionary<IBasicBlock, uint>());
            var bd = Visit(b, 0, new Dictionary<IBasicBlock, uint>());

            return ad.Where(x => bd.ContainsKey(x.Key)).OrderBy(x => x.Value).FirstOrDefault().Key;
        }
    }
}

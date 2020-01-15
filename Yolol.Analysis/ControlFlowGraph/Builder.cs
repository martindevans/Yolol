using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Yolol.Analysis.ControlFlowGraph.AST;
using Yolol.Analysis.TreeVisitor;
using Yolol.Analysis.TreeVisitor.Reduction;
using Yolol.Execution.Extensions;
using Yolol.Grammar.AST;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.ControlFlowGraph
{
    /// <summary>
    /// Build a control flow graph from a program
    /// </summary>
    /// <remarks>
    /// Program must be a simplified form of Yolol:
    ///  + if conditional cannot throw (rewrite to `tmp=cond if tmp then ...` if it can)
    ///  + goto expression cannot throw
    ///  + each expression does exactly one thing
    ///  + no compound statements (rewrite `a+=1` as `a = a + 1`)
    ///  + Assignments must happen in two parts (rewrite `a = expression` as `tmp = expression a = tmp`
    /// </remarks>
    public class Builder
    {
        private readonly Program _program;
        private readonly int _maxLines;
        private readonly Dictionary<long, IMutableBasicBlock> _lineStartBlocks = new Dictionary<long, IMutableBasicBlock>();

        public Builder(Program program, int maxLines)
        {
            _program = program;
            _maxLines = maxLines;
        }

        private void CheckValidity([NotNull] IControlFlowGraph cfg)
        {
            if (!cfg.Vertices.All(v => v.Outgoing.Any()))
                throw new NotImplementedException();
        }

        [NotNull] public IControlFlowGraph Build()
        {
            var cfg = new ControlFlowGraph();

            // Add program entry point into line 1
            var root = cfg.CreateNewBlock(BasicBlockType.Entry, 0);
            cfg.CreateEdge(root, GetLineEntryBlock(cfg, 1), EdgeType.Continue);

            // Rewrite program into simpler form
            var names = new SafeNameGenerator(new SequentialNameGenerator("_"), _program);
            var ast = new ProgramDecomposition(names).Visit(_program);
            ast = new FlattenStatementLists().Visit(ast);

            for (var lineNumber = 1; lineNumber <= _maxLines; lineNumber++)
            {
                var line = lineNumber > ast.Lines.Count ? new Line(new StatementList(Array.Empty<BaseStatement>())) : ast.Lines[lineNumber - 1];
                var block = GetLineEntryBlock(cfg, lineNumber);

                var (_, ex) = HandleStatementList(cfg, line.Statements, lineNumber, block);

                // If necessary, add fallthrough to next line
                if (ex != null)
                    AddFallthrough(cfg, ex, lineNumber);
            }

            return cfg;
        }

        /// <summary>
        /// Convert a statement list into basic blocks. Return the final block that was created if it needs to be connected to the next block
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="statements"></param>
        /// <param name="lineNumber"></param>
        /// <param name="entry"></param>
        /// <returns>(entry_block!, exit_block?)</returns>
        private (IMutableBasicBlock, IMutableBasicBlock) HandleStatementList([NotNull] IMutableControlFlowGraph cfg, [NotNull] StatementList statements, int lineNumber, [NotNull] IMutableBasicBlock entry)
        {
            var block = cfg.CreateNewBlock(BasicBlockType.Basic, lineNumber);
            cfg.CreateEdge(entry, block, EdgeType.Continue);

            foreach (var stmt in statements.Statements)
            {
                if (stmt is Goto @goto)
                {
                    HandleGoto(cfg, @goto, block, lineNumber);

                    // Create a new block which is _not_ linked to the previous (because we just unconditionally jumped away)
                    block = cfg.CreateNewBlock(BasicBlockType.Basic, lineNumber);
                }
                else if (stmt is If @if)
                {
                    block.Add(new Conditional(@if.Condition));

                    // Convert true and false branches into blocks
                    var (enTrue, exTrue) = HandleStatementList(cfg, @if.TrueBranch, lineNumber, cfg.CreateNewBlock(BasicBlockType.Basic, lineNumber));
                    var (enFals, exFals) = HandleStatementList(cfg, @if.FalseBranch, lineNumber, cfg.CreateNewBlock(BasicBlockType.Basic, lineNumber));

                    // Create edges from conditional to two branches
                    cfg.CreateEdge(block, enTrue, EdgeType.ConditionalTrue);
                    cfg.CreateEdge(block, enFals, EdgeType.ConditionalFalse);

                    // Link the exit blocks (if they're not null) to the next block
                    block = cfg.CreateNewBlock(BasicBlockType.Basic, lineNumber);
                    if (exTrue != null)
                        cfg.CreateEdge(exTrue, block, EdgeType.Continue);
                    if (exFals != null)
                        cfg.CreateEdge(exFals, block, EdgeType.Continue);
                    
                }
                else if (stmt is EmptyStatement)
                {
                    // ignore empty statements, they (obviously) don't do anything
                    // ReSharper disable once RedundantJumpStatement
                    continue;
                }
                else
                {
                    // Add to block with fallthrough to next line if it can error
                    block.Add(stmt);
                    if (stmt.CanRuntimeError)
                    {
                        AddFallthrough(cfg, block, lineNumber, EdgeType.RuntimeError);

                        var b2 = cfg.CreateNewBlock(BasicBlockType.Basic, block.LineNumber);
                        cfg.CreateEdge(block, b2, EdgeType.Continue);
                        block = b2;
                    }
                }
            }

            return (entry, block);
        }

        private void HandleGoto([NotNull] IMutableControlFlowGraph cfg, [NotNull] Goto @goto, [NotNull] IMutableBasicBlock block, int lineNumber)
        {
            block.Add(@goto);

            if (@goto.Destination.IsConstant)
            {
                var dest = @goto.Destination.StaticEvaluate();
                if (dest.Type == Execution.Type.Number)
                {
                    // We know exactly where this is going, jump to that line
                    var line = Math.Clamp((int)dest.Number.Value, 1, _maxLines);
                    var destBlock = GetLineEntryBlock(cfg, line);
                    cfg.CreateEdge(block, destBlock, EdgeType.GotoConstNum);
                }
                else if (dest.Type == Execution.Type.String)
                {
                    // We tried to statically jump to a string (which is always an error), fallthrough to the next line
                    AddFallthrough(cfg, block, lineNumber, EdgeType.GotoConstStr);
                }
            }
            else
            {
                // We don't know where this is going, so goto every line
                for (var j = 1; j <= _maxLines; j++)
                    cfg.CreateEdge(block, GetLineEntryBlock(cfg, j), EdgeType.GotoExpression);
            }
        }

        private void AddFallthrough([NotNull] IMutableControlFlowGraph cfg, [NotNull] IBasicBlock source, int currentLineNumber, EdgeType type = EdgeType.Continue)
        {
            cfg.CreateEdge(source, GetLineEntryBlock(cfg, currentLineNumber == _maxLines ? 1 : currentLineNumber + 1), type);
        }

        private IMutableBasicBlock GetLineEntryBlock([NotNull] IMutableControlFlowGraph cfg, int lineNumber)
        {
            if (!_lineStartBlocks.TryGetValue(lineNumber, out var block))
            {
                block = cfg.CreateNewBlock(BasicBlockType.LineStart, lineNumber);
                _lineStartBlocks.Add(lineNumber, block);
            }

            return block;
        }
    }
}

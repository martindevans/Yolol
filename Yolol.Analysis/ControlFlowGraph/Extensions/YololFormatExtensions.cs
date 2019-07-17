using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Yolol.Analysis.ControlFlowGraph.AST;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.ControlFlowGraph.Extensions
{
    public static class YololFormatExtensions
    {
        [NotNull] public static Program ToYolol([NotNull] this IControlFlowGraph cfg)
        {

            

            var results = new List<(int, Line)>();

            var lineStarts = cfg.Vertices.Where(v => v.Type == BasicBlockType.LineStart);
            foreach (var lineStart in lineStarts)
            {
                var l = ConvertLine(lineStart);
                results.Add((lineStart.LineNumber, l));
            }

            return new Program(
                results.OrderBy(a => a.Item1).Select(a => a.Item2)
            );
        }

        [NotNull] private static Line ConvertLine([NotNull] IBasicBlock entry)
        {
            var statements = new List<BaseStatement>();

            var e = entry.Outgoing.Single();
            if (e.Type != EdgeType.Continue)
                throw new InvalidOperationException("LineStart block has an invalid edge");

            if (e.End.LineNumber == e.Start.LineNumber)
                ConvertBlock(e.End, statements);

            return new Line(new StatementList(statements));
        }

        private static void ConvertBlock([NotNull] IBasicBlock block, [NotNull] List<BaseStatement> output)
        {
            output.AddRange(block.Statements);

            // conditional may not throw, so the only edges will be true and false
            if (block.Statements.Last() is Conditional con)
            {
                output.RemoveAt(output.Count - 1);
                output.Add(ConvertConditional(con, out var c));

                if (c != null)
                    ConvertBlock(c, output);

                return;
            }

            // Goto may not throw, so the only edge(s) will be jump(s) away (ending this line)
            if (block.Statements.Last() is Goto)
                return;

            // If there's a continue edge and it continues this line, convert it
            var e = block.Outgoing.SingleOrDefault(a => a.Type == EdgeType.Continue);
            if (e != null && e.Type == EdgeType.Continue && e.End.LineNumber == block.LineNumber)
                ConvertBlock(e.End, output);
        }

        [NotNull] private static BaseStatement ConvertConditional([NotNull] Conditional con, [CanBeNull] out IBasicBlock continueWith)
        {
            //todo: conditional conversion
            continueWith = null;
            return new If(con.Condition, new StatementList(), new StatementList());
        }
    }
}

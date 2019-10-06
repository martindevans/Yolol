using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Yolol.Analysis.TreeVisitor;
using Yolol.Grammar.AST;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.ControlFlowGraph
{
    public interface IBasicBlock
        : IEquatable<IBasicBlock>
    {
        BasicBlockType Type { get; }

        Guid ID { get; }

        int LineNumber { get; }

        IEnumerable<BaseStatement> Statements { get; }

    IEnumerable<IEdge> Incoming { get; }

        IEnumerable<IEdge> Outgoing { get; }
    }

    public interface IMutableBasicBlock
        : IBasicBlock
    {
        void Add(BaseStatement stmt);
    }

    // ReSharper disable once InconsistentNaming
    public static class IBasicBlockExtensions
    {
        [NotNull] public static StatementList Visit([NotNull] this BaseTreeVisitor visitor, [NotNull] IBasicBlock block)
        {
            var result = visitor.Visit(new Program(new Line[] {new Line(new StatementList(block.Statements))}));

            return result.Lines.Single().Statements;
        }

        public static void CopyTo([NotNull] this IBasicBlock a, [NotNull] IMutableBasicBlock b)
        {
            foreach (var stmt in a.Statements)
                b.Add(stmt);
        }
    }
}

using System;
using System.Collections.Generic;
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

        void AddOutgoing(IEdge edge);

        void AddIncoming(IEdge edge);
    }
}

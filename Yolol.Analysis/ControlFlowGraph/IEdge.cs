using System;

namespace Yolol.Analysis.ControlFlowGraph
{
    public interface IEdge
        : IEquatable<IEdge>
    {
        IBasicBlock Start { get; }

        IBasicBlock End { get; }

        EdgeType Type { get; }
    }
}

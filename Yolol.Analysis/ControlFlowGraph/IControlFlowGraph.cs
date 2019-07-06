using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Yolol.Analysis.ControlFlowGraph
{
    public interface IControlFlowGraph
    {
        IEnumerable<IEdge> Edges { get; }

        uint EdgeCount { get; }

        IEnumerable<IBasicBlock> Vertices { get; }

        uint VertexCount { get; }

        [CanBeNull] IBasicBlock Vertex(Guid id);
    }
}

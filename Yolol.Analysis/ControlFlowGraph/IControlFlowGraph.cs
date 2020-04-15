using System;
using System.Collections.Generic;
using System.Linq;

namespace Yolol.Analysis.ControlFlowGraph
{
    public interface IControlFlowGraph
        : IEquatable<IControlFlowGraph>
    {
        IEnumerable<IEdge> Edges { get; }

        uint EdgeCount { get; }

        IEnumerable<IBasicBlock> Vertices { get; }

        uint VertexCount { get; }

        IBasicBlock? Vertex(Guid id);
    }

    public interface IMutableControlFlowGraph
        : IControlFlowGraph
    {
        IMutableBasicBlock CreateNewBlock(BasicBlockType type, int lineNumber, Guid? id = null);

        IEdge CreateEdge(IBasicBlock start, IBasicBlock end, EdgeType type);
    }

    public static class IControlFlowGraphExtensions
    {
        public static IBasicBlock EntryPoint(this IControlFlowGraph cfg)
        {
            return cfg.Vertices.Single(a => a.Type == BasicBlockType.Entry);
        }
    }
}

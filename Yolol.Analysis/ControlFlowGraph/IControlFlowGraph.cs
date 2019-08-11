using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Yolol.Analysis.ControlFlowGraph
{
    public interface IControlFlowGraph
        : IEquatable<IControlFlowGraph>
    {
        IEnumerable<IEdge> Edges { get; }

        uint EdgeCount { get; }

        IEnumerable<IBasicBlock> Vertices { get; }

        uint VertexCount { get; }

        [CanBeNull] IBasicBlock Vertex(Guid id);
    }

    public interface IMutableControlFlowGraph
        : IControlFlowGraph
    {
        IMutableBasicBlock CreateNewBlock(BasicBlockType type, int lineNumber, Guid? id = null);

        IEdge CreateEdge([NotNull] IBasicBlock start, [NotNull] IBasicBlock end, EdgeType type);
    }

    public static class IControlFlowGraphExtensions
    {
        public static IBasicBlock EntryPoint([NotNull] this IControlFlowGraph cfg)
        {
            return cfg.Vertices.Single(a => a.Type == BasicBlockType.Entry);
        }
    }
}

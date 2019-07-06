using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.ControlFlowGraph
{
    public class ControlFlowGraph
        : IControlFlowGraph
    {
        private readonly Dictionary<Guid, BasicBlock> _vertexLookup = new Dictionary<Guid, BasicBlock>();
        private readonly List<Edge> _edges = new List<Edge>();

        public IEnumerable<IEdge> Edges => _edges;

        public uint EdgeCount => (uint)_edges.Count;

        public IEnumerable<IBasicBlock> Vertices => _vertexLookup.Values;

        public uint VertexCount => (uint)_vertexLookup.Count;

        [NotNull] public IMutableBasicBlock CreateNewBlock(BasicBlockType type, int lineNumber, Guid? id = null)
        {
            var block = new BasicBlock(id ?? Guid.NewGuid(), type, lineNumber);

            _vertexLookup.Add(block.ID, block);
            return block;
        }

        [NotNull] public IEdge CreateEdge([NotNull] IMutableBasicBlock start, [NotNull] IMutableBasicBlock end, EdgeType type)
        {
            var e = new Edge(start, end, type);

            start.AddOutgoing(e);
            end.AddIncoming(e);

            _edges.Add(e);

            return e;
        }

        public IBasicBlock Vertex(Guid id)
        {
            _vertexLookup.TryGetValue(id, out var block);
            return block;
        }

        private class BasicBlock
            : IMutableBasicBlock
        {
            public BasicBlockType Type { get; }

            public Guid ID { get; }

            public int LineNumber { get; }

            public IEnumerable<BaseStatement> Statements => _statements;

            public IEnumerable<IEdge> Incoming => _incoming;

            public IEnumerable<IEdge> Outgoing => _outgoing;

            private readonly List<BaseStatement> _statements = new List<BaseStatement>();
            private readonly List<IEdge> _outgoing = new List<IEdge>();
            private readonly List<IEdge> _incoming = new List<IEdge>();

            public BasicBlock(Guid id, BasicBlockType type, int lineNumber)
            {
                ID = id;
                Type = type;
                LineNumber = lineNumber;
            }

            public void Add(BaseStatement stmt)
            {
                _statements.Add(stmt);
            }

            public void AddOutgoing(IEdge edge)
            {
                _outgoing.Add(edge);
            }

            public void AddIncoming(IEdge edge)
            {
                _incoming.Add(edge);
            }

            [NotNull] public override string ToString()
            {
                if (Type == BasicBlockType.Entry)
                    return "Entrypoint";
                else
                    return $"[L{LineNumber}]\\n" + string.Join("\\n", _statements).Replace("\"", "\\\"");
            }

            #region equality
            public bool Equals([CanBeNull] IBasicBlock other)
            {
                if (ReferenceEquals(null, other))
                    return false;
                if (ReferenceEquals(this, other))
                    return true;
                return ID.Equals(other.ID);
            }

            public override int GetHashCode()
            {
                return ID.GetHashCode();
            }
            #endregion
        }

        private class Edge
            : IEdge
        {
            public IBasicBlock Start { get; }

            public IBasicBlock End { get; }

            public EdgeType Type { get; }

            public Edge(IBasicBlock start, IBasicBlock end, EdgeType type)
            {
                Start = start;
                End = end;
                Type = type;
            }

            [NotNull] public override string ToString()
            {
                return $"{Start}=>{End} ({Type})";
            }

            #region equality
            public bool Equals([CanBeNull] IEdge other)
            {
                if (ReferenceEquals(null, other))
                    return false;
                if (ReferenceEquals(this, other))
                    return true;
                return Equals(Start, other.Start) && Equals(End, other.End) && Type == other.Type;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = (Start != null ? Start.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (End != null ? End.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (int)Type;
                    return hashCode;
                }
            }
            #endregion
        }
    }
}

/* a = b++ * c * d++
 * 
 w = b++
 x = w * c -->
 y = d++
 z = x * y
 a = z
 */
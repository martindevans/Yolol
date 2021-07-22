using System;
using System.Collections.Generic;
using Yolol.Grammar.AST.Statements;
using System.Linq;

namespace Yolol.Analysis.ControlFlowGraph
{
    public class ControlFlowGraph
        : IMutableControlFlowGraph
    {
        private readonly Dictionary<Guid, BasicBlock> _vertexLookup = new Dictionary<Guid, BasicBlock>();
        private readonly List<Edge> _edges = new List<Edge>();

        public IEnumerable<IEdge> Edges => _edges;

        public uint EdgeCount => (uint)_edges.Count;

        public IEnumerable<IBasicBlock> Vertices => _vertexLookup.Values;

        public uint VertexCount => (uint)_vertexLookup.Count;

        public IMutableBasicBlock CreateNewBlock(BasicBlockType type, int lineNumber, Guid? id = null)
        {
            var block = new BasicBlock(id ?? Guid.NewGuid(), type, lineNumber);

            _vertexLookup.Add(block.ID, block);
            return block;
        }

        public IEdge CreateEdge(IBasicBlock start, IBasicBlock end, EdgeType type)
        {
            var e = new Edge(start, end, type);

            _vertexLookup[start.ID].AddOutgoing(e);
            _vertexLookup[end.ID].AddIncoming(e);

            _edges.Add(e);

            return e;
        }

        public IBasicBlock Vertex(Guid id)
        {
            _vertexLookup.TryGetValue(id, out var block);
            return block;
        }

        public bool Equals(IControlFlowGraph other)
        {
            if (other is null)
                return false;
            if (other.EdgeCount != EdgeCount || other.VertexCount != VertexCount)
                return false;

            // Check all edges are the same
            if (!other.Edges.OrderBy(a => a.Start.ID).Zip(Edges.OrderBy(a => a.Start.ID), (a, b) => a.Equals(b)).All(a => a))
                return false;

            // Check all vertices are the same
            if (!other.Vertices.OrderBy(a => a.ID).Zip(Vertices.OrderBy(a => a.ID), (a, b) => a.Equals(b)).All(a => a))
                return false;

            return true;
        }

        private interface IExposedMutableBlock
            : IMutableBasicBlock
        {
            /// <summary>
            /// Add a new outgoing edge from this block to another block
            /// </summary>
            /// <param name="edge"></param>
            void AddOutgoing(IEdge edge);

            /// <summary>
            /// Add a new incoming edge to this block from another block
            /// </summary>
            /// <param name="edge"></param>
            void AddIncoming(IEdge edge);
        }

        private class BasicBlock
            : IExposedMutableBlock
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

            public override string ToString()
            {
                if (Type == BasicBlockType.Entry)
                    return "Entrypoint";
                else
                    return $"[L{LineNumber}]\\n" + string.Join("\\n", _statements).Replace("\"", "\\\"");
            }

            #region equality
            public bool Equals(IBasicBlock? other)
            {
                if (other is null)
                    return false;
                if (ReferenceEquals(this, other))
                    return true;

                if (!ID.Equals(other.ID))
                    return false;
                if (_statements.Count != other.Statements.Count())
                    return false;

                return _statements.Zip(other.Statements, (a, b) => a.Equals(b)).All(a => a);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(ID);
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

            public override string ToString()
            {
                return $"{Start}=>{End} ({Type})";
            }

            #region equality
            public bool Equals(IEdge? other)
            {
                if (other is null)
                    return false;
                if (ReferenceEquals(this, other))
                    return true;
                return Start.Equals(other.Start) && End.Equals(other.End) && Type == other.Type;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Start, End, Type);
            }
            #endregion
        }
    }
}
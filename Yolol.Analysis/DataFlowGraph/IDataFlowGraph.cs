using System;
using System.Collections.Generic;
using Yolol.Analysis.ControlFlowGraph;
using Yolol.Analysis.ControlFlowGraph.Extensions;
using Yolol.Execution;
using Yolol.Grammar;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Expressions.Unary;

namespace Yolol.Analysis.DataFlowGraph
{
    public interface IDataFlowGraph
    {
        IBasicBlock Block { get; }

        ISingleStaticAssignmentTable SSA { get; }

        IEnumerable<IDataFlowGraphInput> Inputs { get; }

        IEnumerable<IDataFlowGraphOp> Operations { get; }

        IEnumerable<IDataFlowGraphOutput> Outputs { get; }
    }

    public interface IDataFlowGraphNode
    {
        Guid Id { get; }
    }

    public enum DataFlowGraphInputType
    {
        Constant,
        Variable
    }

    public interface IDataFlowGraphInput
        : IDataFlowGraphNode
    {
        DataFlowGraphInputType Type { get; }
    }

    public interface IDataFlowGraphInputConstant
        : IDataFlowGraphInput
    {
        Value Value { get; }
    }

    public interface IDataFlowGraphInputVariable
        : IDataFlowGraphInput
    {
        VariableName Name { get; }
    }

    public interface IDataFlowGraphOp
        : IDataFlowGraphNode
    {
        string Label { get; }

        IEnumerable<IDataFlowGraphNode> Inputs { get; }

        BaseExpression Expression { get; }
    }

    public interface IDataFlowGraphOutput
        : IDataFlowGraphNode
    {
        IDataFlowGraphNode Input { get; }

        VariableName Name { get; }
    }
}

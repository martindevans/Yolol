using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Yolol.Analysis.ControlFlowGraph;
using Yolol.Analysis.ControlFlowGraph.Extensions;
using Yolol.Execution;
using Yolol.Grammar;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Statements;

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

        [NotNull] BaseExpression ToExpression();
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
        IEnumerable<IDataFlowGraphNode> Inputs { get; }

        [NotNull] BaseExpression ToExpression();
    }

    public interface IDataFlowGraphOutput
        : IDataFlowGraphNode
    {
        IDataFlowGraphNode Input { get; }

        [NotNull] BaseStatement ToStatement();
    }
}

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Yolol.Analysis.ControlFlowGraph.Extensions;
using Yolol.Execution;
using Yolol.Grammar;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.DataFlowGraph
{
    public interface IDataFlowGraph
    {
        ISingleStaticAssignmentTable Ssa { get; }

        IEnumerable<IDataFlowGraphInput> Inputs { get; }

        IEnumerable<IDataFlowGraphOp> Operations { get; }

        IEnumerable<IDataFlowGraphOutput> Outputs { get; }
    }

    public interface IDataFlowGraphNode
    {
        Guid Id { get; }
    }

    public interface IDataFlowGraphExpressionNode
        : IDataFlowGraphNode
    {
        [NotNull] BaseExpression ToExpression();

        [NotNull] IEnumerable<IDataFlowGraphNode> Inputs { get; }
    }

    public enum DataFlowGraphInputType
    {
        Constant,
        Variable
    }

    public interface IDataFlowGraphInput
        : IDataFlowGraphExpressionNode
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
        : IDataFlowGraphExpressionNode
    {
    }

    public interface IDataFlowGraphOutput
        : IDataFlowGraphExpressionNode
    {
        IDataFlowGraphExpressionNode Input { get; }

        [NotNull] BaseStatement ToStatement();
    }
}

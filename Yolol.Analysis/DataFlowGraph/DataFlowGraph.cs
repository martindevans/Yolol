using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Yolol.Analysis.ControlFlowGraph;
using Yolol.Analysis.ControlFlowGraph.AST;
using Yolol.Analysis.ControlFlowGraph.Extensions;
using Yolol.Analysis.TreeVisitor;
using Yolol.Analysis.TreeVisitor.Inspection;
using Yolol.Execution;
using Yolol.Grammar;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Expressions.Binary;
using Yolol.Grammar.AST.Expressions.Unary;
using Yolol.Grammar.AST.Statements;
using Variable = Yolol.Grammar.AST.Expressions.Unary.Variable;

namespace Yolol.Analysis.DataFlowGraph
{
    public class DataFlowGraph
        : IDataFlowGraph
    {
        public IBasicBlock Block { get; }

        public ISingleStaticAssignmentTable SSA { get; }

        private readonly List<IDataFlowGraphInput> _inputs = new List<IDataFlowGraphInput>();
        public IEnumerable<IDataFlowGraphInput> Inputs => _inputs;

        private readonly List<DataFlowGraphOp> _ops = new List<DataFlowGraphOp>();
        public IEnumerable<IDataFlowGraphOp> Operations => _ops;


        private readonly List<DataFlowGraphOutput> _outputs = new List<DataFlowGraphOutput>();
        public IEnumerable<IDataFlowGraphOutput> Outputs => _outputs;
        

        public DataFlowGraph([NotNull] IBasicBlock block, [NotNull] ISingleStaticAssignmentTable ssa)
        {
            Block = block;
            SSA = ssa;

            // Find read and written variables
            var assigned = new FindAssignedVariables();
            assigned.Visit(block);
            var read = new FindReadVariables();
            read.Visit(block);

            // Convert statements
            foreach (var stmt in block.Statements)
            {
                if (stmt is Assignment ass)
                    _outputs.Add(new DataFlowGraphOutput(ass.Left, new ExpressionConverter(this).Visit(ass.Right), Guid.NewGuid()));
                else if (stmt is Conditional con)
                    _outputs.Add(new DataFlowGraphOutput(new VariableName("conditional"), new ExpressionConverter(this).Visit(con.Condition), Guid.NewGuid()));
                else if (stmt is Goto @goto)
                    _outputs.Add(new DataFlowGraphOutput(new VariableName("goto"), new ExpressionConverter(this).Visit(@goto.Destination), Guid.NewGuid()));
                else
                    throw new NotSupportedException(stmt.GetType().Name);
            }
        }

        private class DataFlowGraphOutput
            : IDataFlowGraphOutput
        {
            public Guid Id { get; }

            public IDataFlowGraphNode Input { get; }

            public VariableName Name { get; }

            public DataFlowGraphOutput(VariableName name, IDataFlowGraphNode input, Guid id)
            {
                Name = name;
                Input = input;
                Id = id;
            }
        }

        private class InputVariable
            : IDataFlowGraphInputVariable
        {
            public Guid Id { get; }
            public DataFlowGraphInputType Type => DataFlowGraphInputType.Variable;
            public VariableName Name { get; }

            public InputVariable(Guid id, VariableName name)
            {
                Id = id;
                Name = name;
            }
        }

        private class InputConst
            : IDataFlowGraphInputConstant
        {
            public Guid Id { get; }
            public DataFlowGraphInputType Type => DataFlowGraphInputType.Constant;
            public Value Value { get; }

            public InputConst(Guid id, Value value)
            {
                Id = id;
                Value = value;
            }

            
        }

        private class DataFlowGraphOp
            : IDataFlowGraphOp
        {
            public string Label { get; }

            public Guid Id { get; }

            private readonly List<IDataFlowGraphNode> _inputs = new List<IDataFlowGraphNode>();
            public IEnumerable<IDataFlowGraphNode> Inputs => _inputs;

            public BaseExpression Expression => throw new NotImplementedException();

            public DataFlowGraphOp(string label, Guid id)
            {
                Label = label;
                Id = id;
            }

            public void AddInput(IDataFlowGraphNode node)
            {
                _inputs.Add(node);
            }
        }

        private class ExpressionConverter
            : BaseExpressionVisitor<IDataFlowGraphNode>
        {
            private readonly DataFlowGraph _dataFlowGraph;

            public ExpressionConverter(DataFlowGraph dataFlowGraph)
            {
                _dataFlowGraph = dataFlowGraph;
            }

            [NotNull] private DataFlowGraphOp VisitBinary([NotNull] BaseBinaryExpression bin, string label)
            {
                var l = Visit(bin.Left);
                var r = Visit(bin.Right);

                var op = new DataFlowGraphOp(label, Guid.NewGuid());
                op.AddInput(l);
                op.AddInput(r);

                _dataFlowGraph._ops.Add(op);
                return op;
            }

            protected override IDataFlowGraphNode Visit(Or or) => VisitBinary(or, "or");

            protected override IDataFlowGraphNode Visit(And and) => VisitBinary(and, "and");

            protected override IDataFlowGraphNode Visit(ErrorExpression err)
            {
                throw new NotImplementedException();
            }

            protected override IDataFlowGraphNode Visit(Increment inc)
            {
                throw new NotImplementedException();
            }

            protected override IDataFlowGraphNode Visit(Decrement dec)
            {
                throw new NotImplementedException();
            }

            protected override IDataFlowGraphNode Visit(Phi phi)
            {
                var op = new DataFlowGraphOp("ϕ", Guid.NewGuid());

                foreach (var name in phi.AssignedNames)
                {
                    var v = Visit(new Variable(name));
                    op.AddInput(v);
                }

                _dataFlowGraph._ops.Add(op);
                return op;
            }

            protected override IDataFlowGraphNode Visit(LessThanEqualTo eq) => VisitBinary(eq, "<=");

            protected override IDataFlowGraphNode Visit(LessThan eq) => VisitBinary(eq, "<");

            protected override IDataFlowGraphNode Visit(GreaterThanEqualTo eq) => VisitBinary(eq, ">=");

            protected override IDataFlowGraphNode Visit(GreaterThan eq) => VisitBinary(eq, ">");

            protected override IDataFlowGraphNode Visit(NotEqualTo eq) => VisitBinary(eq, "!=");

            protected override IDataFlowGraphNode Visit(EqualTo eq) => VisitBinary(eq, "==");

            protected override IDataFlowGraphNode Visit(Variable var)
            {
                // If this is an external variable add a new input node
                if (var.Name.IsExternal)
                {
                    var external = new InputVariable(Guid.NewGuid(), var.Name);
                    _dataFlowGraph._inputs.Add(external);
                    return external;
                }

                // It's not external, try to find an output node which was previously assigned
                var output = _dataFlowGraph._outputs.SingleOrDefault(a => a.Name.Equals(var.Name));
                if (output != null)
                    return output;

                // It wasn't previously assigned, add a new input
                var input = new InputVariable(Guid.NewGuid(), var.Name);
                _dataFlowGraph._inputs.Add(input);
                return input;
            }

            protected override IDataFlowGraphNode Visit(Modulo mod) => VisitBinary(mod, "%");

            protected override IDataFlowGraphNode Visit(PreDecrement dec)
            {
                throw new NotImplementedException();
            }

            protected override IDataFlowGraphNode Visit(PostDecrement dec)
            {
                throw new NotImplementedException();
            }

            protected override IDataFlowGraphNode Visit(PreIncrement inc)
            {
                throw new NotImplementedException();
            }

            protected override IDataFlowGraphNode Visit(PostIncrement inc)
            {
                throw new NotImplementedException();
            }

            protected override IDataFlowGraphNode Visit(Application app)
            {
                throw new NotImplementedException();
            }

            protected override IDataFlowGraphNode Visit(Bracketed brk)
            {
                throw new NotImplementedException();
            }

            protected override IDataFlowGraphNode Visit(Add add) => VisitBinary(add, "+");

            protected override IDataFlowGraphNode Visit(Subtract sub) => VisitBinary(sub, "-");

            protected override IDataFlowGraphNode Visit(Multiply mul) => VisitBinary(mul, "*");

            protected override IDataFlowGraphNode Visit(Divide div) => VisitBinary(div, "/");

            protected override IDataFlowGraphNode Visit(Exponent exp) => VisitBinary(exp, "^");

            protected override IDataFlowGraphNode Visit(Negate neg)
            {
                throw new NotImplementedException();
            }

            protected override IDataFlowGraphNode Visit(ConstantNumber con)
            {

                var op = new InputConst(Guid.NewGuid(), con.Value.Value);
                _dataFlowGraph._inputs.Add(op);
                return op;
            }

            protected override IDataFlowGraphNode Visit(ConstantString con)
            {
                var op = new InputConst(Guid.NewGuid(), con.Value);
                _dataFlowGraph._inputs.Add(op);
                return op;
            }
        }
    }
}

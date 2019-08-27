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

        private readonly List<IDataFlowGraphOp> _ops = new List<IDataFlowGraphOp>();
        public IEnumerable<IDataFlowGraphOp> Operations => _ops;


        private readonly List<IDataFlowGraphOutput> _outputs = new List<IDataFlowGraphOutput>();
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
                    _outputs.Add(new AssignmentOutput(ass.Left, new ExpressionConverter(this).Visit(ass.Right), Guid.NewGuid()));
                else if (stmt is Conditional con)
                    _outputs.Add(new ConditionalOutput(new ExpressionConverter(this).Visit(con.Condition), Guid.NewGuid()));
                else if (stmt is Goto @goto)
                    _outputs.Add(new GotoOutput(new ExpressionConverter(this).Visit(@goto.Destination), Guid.NewGuid()));
                else
                    throw new NotSupportedException(stmt.GetType().Name);
            }
        }

        private interface IExpressionNode
            : IDataFlowGraphNode
        {
            [NotNull] BaseExpression ToExpression();
        }


        private class ConditionalOutput
            : IDataFlowGraphOutput
        {
            public Guid Id { get; }

            public IDataFlowGraphNode Input { get; }

            public ConditionalOutput(IDataFlowGraphNode input, Guid id)
            {
                Input = input;
                Id = id;
            }

            public BaseStatement ToStatement()
            {
                return new Conditional(((IExpressionNode)Input).ToExpression());
            }
        }

        private class AssignmentOutput
            : IDataFlowGraphOutput, IExpressionNode
        {
            public Guid Id { get; }

            public IDataFlowGraphNode Input { get; }

            public VariableName Name { get; }

            public AssignmentOutput(VariableName name, IDataFlowGraphNode input, Guid id)
            {
                Name = name;
                Input = input;
                Id = id;
            }

            public BaseStatement ToStatement()
            {
                return new Assignment(Name, ((IExpressionNode)Input).ToExpression());
            }

            public BaseExpression ToExpression()
            {
                return new Variable(Name);
            }
        }

        private class GotoOutput
            : IDataFlowGraphOutput
        {
            public Guid Id { get; }

            public IDataFlowGraphNode Input { get; }

            public GotoOutput(IDataFlowGraphNode input, Guid id)
            {
                Input = input;
                Id = id;
            }

            public BaseStatement ToStatement()
            {
                return new Goto(((IExpressionNode)Input).ToExpression());
            }
        }




        private class InputVariable
            : IDataFlowGraphInputVariable, IExpressionNode
        {
            public Guid Id { get; }
            public DataFlowGraphInputType Type => DataFlowGraphInputType.Variable;
            public VariableName Name { get; }

            public InputVariable(Guid id, VariableName name)
            {
                Id = id;
                Name = name;
            }

            public BaseExpression ToExpression()
            {
                return new Variable(Name);
            }
        }

        private class InputConst
            : IDataFlowGraphInputConstant, IExpressionNode
        {
            public Guid Id { get; }
            public DataFlowGraphInputType Type => DataFlowGraphInputType.Constant;
            
            public Value Value { get; }

            public InputConst(Guid id, Value value)
            {
                Id = id;
                Value = value;
            }

            public BaseExpression ToExpression()
            {
                if (Value.Type == Execution.Type.Number)
                    return new ConstantNumber(Value.Number);
                else if (Value.Type == Execution.Type.String)
                    return new ConstantString(Value.String);
                else
                    throw new InvalidOperationException($"Invalid constant type {Value.Type}");
            }
        }

        private class PhiOp
            : IDataFlowGraphOp, IExpressionNode
        {
            private readonly IReadOnlyList<IDataFlowGraphNode> _names;
            private readonly ISingleStaticAssignmentTable _ssa;

            public Guid Id { get; }

            public IEnumerable<IDataFlowGraphNode> Inputs => _names;

            public PhiOp(Guid id, IReadOnlyList<IDataFlowGraphNode> names, ISingleStaticAssignmentTable ssa)
            {
                Id = id;
                _names = names;
                _ssa = ssa;

                throw new NotImplementedException();
            }

            public BaseExpression ToExpression()
            {
                throw new NotImplementedException();
                //return new Phi(_ssa, _names);
            }
        }

        private class BinaryOp
            : IDataFlowGraphOp, IExpressionNode
        {
            private YololBinaryOp Op { get; }

            public Guid Id { get; }

            private readonly IExpressionNode[] _inputs = new IExpressionNode[2];
            public IEnumerable<IDataFlowGraphNode> Inputs => _inputs;

            public BinaryOp(YololBinaryOp op, Guid id, IExpressionNode left, IExpressionNode right)
            {
                Op = op;
                Id = id;

                _inputs[0] = left;
                _inputs[1] = right;
            }

            public BaseExpression ToExpression()
            {
                var left = _inputs[0].ToExpression();
                var right = _inputs[1].ToExpression();

                return Op.ToExpression(left, right);
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

            [NotNull] private BinaryOp VisitBinary([NotNull] BaseBinaryExpression bin, YololBinaryOp binop)
            {
                var l = Visit(bin.Left);
                var r = Visit(bin.Right);

                var op = new BinaryOp(binop, Guid.NewGuid(), (IExpressionNode)l, (IExpressionNode)r);

                _dataFlowGraph._ops.Add(op);
                return op;
            }

            protected override IDataFlowGraphNode Visit(Or or) => VisitBinary(or, YololBinaryOp.Or);

            protected override IDataFlowGraphNode Visit(And and) => VisitBinary(and, YololBinaryOp.And);

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
                throw new NotImplementedException();

                //var names = phi.AssignedNames.Select(n => Visit(new Variable(n))).ToArray();

                //var op = new PhiOp(Guid.NewGuid(), names);

                //_dataFlowGraph._ops.Add(op);
                //return op;
            }

            protected override IDataFlowGraphNode Visit(LessThanEqualTo eq) => VisitBinary(eq, YololBinaryOp.LessThanEqualTo);

            protected override IDataFlowGraphNode Visit(LessThan eq) => VisitBinary(eq, YololBinaryOp.LessThan);

            protected override IDataFlowGraphNode Visit(GreaterThanEqualTo eq) => VisitBinary(eq, YololBinaryOp.GreaterThanEqualTo);

            protected override IDataFlowGraphNode Visit(GreaterThan eq) => VisitBinary(eq, YololBinaryOp.GreaterThan);

            protected override IDataFlowGraphNode Visit(NotEqualTo eq) => VisitBinary(eq, YololBinaryOp.NotEqualTo);

            protected override IDataFlowGraphNode Visit(EqualTo eq) => VisitBinary(eq, YololBinaryOp.EqualTo);

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
                var output = _dataFlowGraph._outputs.OfType<AssignmentOutput>().SingleOrDefault(a => a.Name.Equals(var.Name));
                if (output != null)
                    return output;

                // It wasn't previously assigned, add a new input
                var input = new InputVariable(Guid.NewGuid(), var.Name);
                _dataFlowGraph._inputs.Add(input);
                return input;
            }

            protected override IDataFlowGraphNode Visit(Modulo mod) => VisitBinary(mod, YololBinaryOp.Modulo);

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

            protected override IDataFlowGraphNode Visit(Add add) => VisitBinary(add, YololBinaryOp.Add);

            protected override IDataFlowGraphNode Visit(Subtract sub) => VisitBinary(sub, YololBinaryOp.Subtract);

            protected override IDataFlowGraphNode Visit(Multiply mul) => VisitBinary(mul, YololBinaryOp.Multiply);

            protected override IDataFlowGraphNode Visit(Divide div) => VisitBinary(div, YololBinaryOp.Divide);

            protected override IDataFlowGraphNode Visit(Exponent exp) => VisitBinary(exp, YololBinaryOp.Exponent);

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

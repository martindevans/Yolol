using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Yolol.Analysis.ControlFlowGraph;
using Yolol.Analysis.ControlFlowGraph.AST;
using Yolol.Analysis.ControlFlowGraph.Extensions;
using Yolol.Analysis.TreeVisitor;
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
        public ISingleStaticAssignmentTable Ssa { get; }

        private readonly List<IDataFlowGraphInput> _inputs = new List<IDataFlowGraphInput>();
        public IEnumerable<IDataFlowGraphInput> Inputs => _inputs;

        private readonly List<IDataFlowGraphOp> _ops = new List<IDataFlowGraphOp>();
        public IEnumerable<IDataFlowGraphOp> Operations => _ops;

        private readonly List<IDataFlowGraphOutput> _outputs = new List<IDataFlowGraphOutput>();
        public IEnumerable<IDataFlowGraphOutput> Outputs => _outputs;

        public DataFlowGraph([NotNull] ISingleStaticAssignmentTable ssa)
        {
            Ssa = ssa;
        }

        public DataFlowGraph([NotNull] IBasicBlock block, [NotNull] ISingleStaticAssignmentTable ssa)
            : this(ssa)
        {
            // Convert statements
            foreach (var stmt in block.Statements)
            {
                if (stmt is Assignment ass)
                    AddAssignment(ass.Left, Add(ass.Right));
                else if (stmt is Conditional con)
                    AddConditional(Add(con.Condition));
                else if (stmt is Goto @goto)
                    AddGoto(Add(@goto.Destination));
                else
                    throw new NotSupportedException(stmt.GetType().Name);
            }
        }

        [NotNull] public IDataFlowGraphExpressionNode Add([NotNull] BaseExpression expression)
        {
            return new ExpressionConverter(this).Visit(expression);
        }

        [NotNull] public IDataFlowGraphOutput AddAssignment(VariableName name, IDataFlowGraphExpressionNode value)
        {
            var node = new AssignmentOutput(name, value, Guid.NewGuid());
            _outputs.Add(node);
            return node;
        }

        [NotNull] public IDataFlowGraphOutput AddConditional(IDataFlowGraphExpressionNode value)
        {
            var node = new ConditionalOutput(value, Guid.NewGuid());
            _outputs.Add(node);
            return node;
        }

        [NotNull] public IDataFlowGraphOutput AddGoto(IDataFlowGraphExpressionNode value)
        {
            var node = new GotoOutput(value, Guid.NewGuid());
            _outputs.Add(node);
            return node;
        }

        private class ConditionalOutput
            : IDataFlowGraphOutput
        {
            public Guid Id { get; }

            public IDataFlowGraphExpressionNode Input { get; }

            IEnumerable<IDataFlowGraphNode> IDataFlowGraphExpressionNode.Inputs => new[] { Input };

            public ConditionalOutput(IDataFlowGraphExpressionNode input, Guid id)
            {
                Input = input;
                Id = id;
            }

            public BaseStatement ToStatement()
            {
                return new Conditional(Input.ToExpression());
            }

            public BaseExpression ToExpression()
            {
                throw new NotSupportedException();
            }

            

            public override string ToString()
            {
                return "conditional";
            }
        }

        private class AssignmentOutput
            : IDataFlowGraphOutput
        {
            public Guid Id { get; }
            
            public IDataFlowGraphExpressionNode Input { get; }

            IEnumerable<IDataFlowGraphNode> IDataFlowGraphExpressionNode.Inputs => new[] { Input };

            public VariableName Name { get; }

            public AssignmentOutput(VariableName name, IDataFlowGraphExpressionNode input, Guid id)
            {
                Name = name;
                Input = input;
                Id = id;
            }

            public BaseStatement ToStatement()
            {
                return new Assignment(Name, Input.ToExpression());
            }

            public BaseExpression ToExpression()
            {
                return new Variable(Name);
            }

            public override string ToString()
            {
                return Name.ToString();
            }
        }

        private class GotoOutput
            : IDataFlowGraphOutput
        {
            public Guid Id { get; }

            public IDataFlowGraphExpressionNode Input { get; }

            IEnumerable<IDataFlowGraphNode> IDataFlowGraphExpressionNode.Inputs => new[] { Input };

            public GotoOutput(IDataFlowGraphExpressionNode input, Guid id)
            {
                Input = input;
                Id = id;
            }

            public BaseStatement ToStatement()
            {
                return new Goto(Input.ToExpression());
            }

            public BaseExpression ToExpression()
            {
                throw new NotSupportedException();
            }

            public override string ToString()
            {
                return "goto";
            }
        }

        private class InputVariable
            : IDataFlowGraphInputVariable
        {
            public Guid Id { get; }
            public DataFlowGraphInputType Type => DataFlowGraphInputType.Variable;
            public VariableName Name { get; }

            IEnumerable<IDataFlowGraphNode> IDataFlowGraphExpressionNode.Inputs => Array.Empty<IDataFlowGraphNode>();

            public InputVariable(Guid id, VariableName name)
            {
                Id = id;
                Name = name;
            }

            public BaseExpression ToExpression()
            {
                return new Variable(Name);
            }

            public override string ToString()
            {
                return Name.ToString();
            }
        }

        private class InputConst
            : IDataFlowGraphInputConstant
        {
            public Guid Id { get; }
            public DataFlowGraphInputType Type => DataFlowGraphInputType.Constant;
            
            public Value Value { get; }

            IEnumerable<IDataFlowGraphNode> IDataFlowGraphExpressionNode.Inputs => Array.Empty<IDataFlowGraphNode>();

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

            public override string ToString()
            {
                return Value.ToString();
            }
        }

        private class PhiOp
            : IDataFlowGraphOp
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
            }

            [NotNull] private Phi Phi()
            {
                return new Phi(_ssa, _names.Cast<IDataFlowGraphInputVariable>().Select(a => a.Name).ToArray());
            }

            public BaseExpression ToExpression()
            {
                return Phi();
            }

            public override string ToString()
            {
                return Phi().ToString();
            }
        }

        private class BinaryOp
            : IDataFlowGraphOp
        {
            private YololBinaryOp Op { get; }

            public Guid Id { get; }

            private readonly IDataFlowGraphExpressionNode[] _inputs = new IDataFlowGraphExpressionNode[2];
            public IEnumerable<IDataFlowGraphNode> Inputs => _inputs;

            public BinaryOp(YololBinaryOp op, Guid id, IDataFlowGraphExpressionNode left, IDataFlowGraphExpressionNode right)
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

            public override string ToString()
            {
                return Op.String();
            }
        }

        private class UnaryOp
            : IDataFlowGraphOp
        {
            private readonly string _name;
            private readonly Func<BaseExpression, BaseExpression> _toExpression;

            public Guid Id { get; }

            private readonly IDataFlowGraphExpressionNode[] _inputs = new IDataFlowGraphExpressionNode[1];
            public IEnumerable<IDataFlowGraphNode> Inputs => _inputs;

            public UnaryOp(Guid id, string name, IDataFlowGraphExpressionNode input, Func<BaseExpression, BaseExpression> toExpression)
            {
                _name = name;
                _toExpression = toExpression;
                _inputs[0] = input;

                Id = id;
            }

            public override string ToString()
            {
                return _name;
            }

            public BaseExpression ToExpression()
            {
                var input = _inputs[0].ToExpression();
                return _toExpression(input);
            }
        }

        private class ExpressionConverter
            : BaseExpressionVisitor<IDataFlowGraphExpressionNode>
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

                var op = new BinaryOp(binop, Guid.NewGuid(), (IDataFlowGraphExpressionNode)l, (IDataFlowGraphExpressionNode)r);

                _dataFlowGraph._ops.Add(op);
                return op;
            }

            [NotNull] private UnaryOp VisitUnary([NotNull] UnaryOp op)
            {
                _dataFlowGraph._ops.Add(op);
                return op;
            }

            protected override IDataFlowGraphExpressionNode Visit(Or or) => VisitBinary(or, YololBinaryOp.Or);

            protected override IDataFlowGraphExpressionNode Visit(And and) => VisitBinary(and, YololBinaryOp.And);

            protected override IDataFlowGraphExpressionNode Visit(ErrorExpression err)
            {
                throw new NotImplementedException();
            }

            protected override IDataFlowGraphExpressionNode Visit(Increment inc)
            {
                return VisitUnary(new UnaryOp(Guid.NewGuid(), "++", Visit(new Variable(inc.Name)), a => new Increment(((Variable)a).Name)));
            }

            protected override IDataFlowGraphExpressionNode Visit(Decrement dec)
            {
                return VisitUnary(new UnaryOp(Guid.NewGuid(), "--", Visit(new Variable(dec.Name)), a => new Decrement(((Variable)a).Name)));
            }

            protected override IDataFlowGraphExpressionNode Visit(Phi phi)
            {
                var names = phi.AssignedNames.Select(n => Visit(new Variable(n))).ToArray();

                var op = new PhiOp(Guid.NewGuid(), names, phi.SSA);

                _dataFlowGraph._ops.Add(op);
                return op;
            }

            protected override IDataFlowGraphExpressionNode Visit(LessThanEqualTo eq) => VisitBinary(eq, YololBinaryOp.LessThanEqualTo);

            protected override IDataFlowGraphExpressionNode Visit(LessThan eq) => VisitBinary(eq, YololBinaryOp.LessThan);

            protected override IDataFlowGraphExpressionNode Visit(GreaterThanEqualTo eq) => VisitBinary(eq, YololBinaryOp.GreaterThanEqualTo);

            protected override IDataFlowGraphExpressionNode Visit(GreaterThan eq) => VisitBinary(eq, YololBinaryOp.GreaterThan);

            protected override IDataFlowGraphExpressionNode Visit(NotEqualTo eq) => VisitBinary(eq, YololBinaryOp.NotEqualTo);

            protected override IDataFlowGraphExpressionNode Visit(EqualTo eq) => VisitBinary(eq, YololBinaryOp.EqualTo);

            protected override IDataFlowGraphExpressionNode Visit(Variable var)
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

            protected override IDataFlowGraphExpressionNode Visit(Modulo mod) => VisitBinary(mod, YololBinaryOp.Modulo);

            protected override IDataFlowGraphExpressionNode Visit(PreDecrement dec)
            {
                throw new NotSupportedException("PreDecrement must be converted to Decrement before data flow analysis");
            }

            protected override IDataFlowGraphExpressionNode Visit(PostDecrement dec)
            {
                throw new NotSupportedException("PostDecrement must be converted to Decrement before data flow analysis");
            }

            protected override IDataFlowGraphExpressionNode Visit(PreIncrement inc)
            {
                throw new NotSupportedException("PreIncrement must be converted to Increment before data flow analysis");
            }

            protected override IDataFlowGraphExpressionNode Visit(PostIncrement inc)
            {
                throw new NotSupportedException("PostIncrement must be converted to Increment before data flow analysis");
            }

            protected override IDataFlowGraphExpressionNode Visit(Abs app)
            {
                return VisitUnary(new UnaryOp(Guid.NewGuid(), "Abs", Visit(app.Parameter), a => new Abs(a)));
            }

            protected override IDataFlowGraphExpressionNode Visit(Sqrt app)
            {
                return VisitUnary(new UnaryOp(Guid.NewGuid(), "sqrt", Visit(app.Parameter), a => new Sqrt(a)));
            }

            protected override IDataFlowGraphExpressionNode Visit(Sine app)
            {
                return VisitUnary(new UnaryOp(Guid.NewGuid(), "sin", Visit(app.Parameter), a => new Sine(a)));
            }

            protected override IDataFlowGraphExpressionNode Visit(Cosine app)
            {
                return VisitUnary(new UnaryOp(Guid.NewGuid(), "cos", Visit(app.Parameter), a => new Cosine(a)));
            }

            protected override IDataFlowGraphExpressionNode Visit(Tangent app)
            {
                return VisitUnary(new UnaryOp(Guid.NewGuid(), "tan", Visit(app.Parameter), a => new Tangent(a)));
            }

            protected override IDataFlowGraphExpressionNode Visit(ArcSine app)
            {
                return VisitUnary(new UnaryOp(Guid.NewGuid(), "asin", Visit(app.Parameter), a => new ArcSine(a)));
            }

            protected override IDataFlowGraphExpressionNode Visit(ArcCos app)
            {
                return VisitUnary(new UnaryOp(Guid.NewGuid(), "acos", Visit(app.Parameter), a => new ArcCos(a)));
            }

            protected override IDataFlowGraphExpressionNode Visit(ArcTan app)
            {
                return VisitUnary(new UnaryOp(Guid.NewGuid(), "atan", Visit(app.Parameter), a => new ArcTan(a)));
            }

            protected override IDataFlowGraphExpressionNode Visit(Bracketed brk)
            {
                return VisitUnary(new UnaryOp(Guid.NewGuid(), "()", Visit(brk.Expression), a => new Bracketed(a)));
            }

            protected override IDataFlowGraphExpressionNode Visit(Add add) => VisitBinary(add, YololBinaryOp.Add);

            protected override IDataFlowGraphExpressionNode Visit(Subtract sub) => VisitBinary(sub, YololBinaryOp.Subtract);

            protected override IDataFlowGraphExpressionNode Visit(Multiply mul) => VisitBinary(mul, YololBinaryOp.Multiply);

            protected override IDataFlowGraphExpressionNode Visit(Divide div) => VisitBinary(div, YololBinaryOp.Divide);

            protected override IDataFlowGraphExpressionNode Visit(Exponent exp) => VisitBinary(exp, YololBinaryOp.Exponent);

            protected override IDataFlowGraphExpressionNode Visit(Negate neg)
            {
                return VisitUnary(new UnaryOp(Guid.NewGuid(), "negate", Visit(neg.Expression), a => new Negate(a)));
            }

            protected override IDataFlowGraphExpressionNode Visit(ConstantNumber con)
            {

                var op = new InputConst(Guid.NewGuid(), con.Value.Value);
                _dataFlowGraph._inputs.Add(op);
                return op;
            }

            protected override IDataFlowGraphExpressionNode Visit(ConstantString con)
            {
                var op = new InputConst(Guid.NewGuid(), con.Value);
                _dataFlowGraph._inputs.Add(op);
                return op;
            }
        }
    }
}

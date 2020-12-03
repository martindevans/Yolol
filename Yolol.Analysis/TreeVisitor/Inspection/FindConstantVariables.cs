using System;
using System.Collections.Generic;
using System.Linq;
using Yolol.Analysis.ControlFlowGraph.AST;
using Yolol.Analysis.ControlFlowGraph.Extensions;
using Yolol.Grammar;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Expressions.Binary;
using Yolol.Grammar.AST.Expressions.Unary;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.TreeVisitor.Inspection
{
    public class FindConstantVariables
        : BaseTreeVisitor
    {
        private readonly Dictionary<VariableName, BaseExpression> _constants;

        // ReSharper disable once NotAccessedField.Local (this field is included in the constructor as a hint that SSA form is required)
#pragma warning disable IDE0052 // Remove unread private members
        private readonly ISingleStaticAssignmentTable _ssa;
#pragma warning restore IDE0052 // Remove unread private members

        public FindConstantVariables(Dictionary<VariableName, BaseExpression> constants, ISingleStaticAssignmentTable ssa)
        {
            _constants = constants;
            _ssa = ssa;
        }

        protected override BaseStatement Visit(TypedAssignment ass)
        {
            if (!ass.Left.IsExternal && new ExpressionConstantFinder(_constants).Visit(ass.Right))
            {
                if (_constants.ContainsKey(ass.Left))
                {
                    if (!_constants[ass.Left].Equals(ass.Right))
                        throw new InvalidOperationException("Variable has already been marked constant with a different value (variable is assigned multiple times, SSA form not correct");
                }
                else
                    _constants.Add(ass.Left, ass.Right);
            }

            return base.Visit(ass);
        }

        protected override BaseStatement Visit(Assignment ass)
        {
            if (!ass.Left.IsExternal && new ExpressionConstantFinder(_constants).Visit(ass.Right))
                _constants.Add(ass.Left, ass.Right);

            return base.Visit(ass);
        }
    }

    public class ExpressionConstantFinder
        : BaseExpressionVisitor<bool>
    {
        private readonly IReadOnlyDictionary<VariableName, BaseExpression> _constants;

        public ExpressionConstantFinder(IReadOnlyDictionary<VariableName, BaseExpression> constants)
        {
            _constants = constants;
        }

        private bool VisitBinary(BaseBinaryExpression bin) => Visit(bin.Left) && Visit(bin.Right);

        protected override bool Visit(Or or) => VisitBinary(or);

        protected override bool Visit(And and) => VisitBinary(and);

        protected override bool Visit(Not not) => Visit(not.Parameter);

        protected override bool Visit(ErrorExpression err) => true;

        protected override bool Visit(Increment inc) => _constants.ContainsKey(inc.Name);

        protected override bool Visit(Decrement dec) => _constants.ContainsKey(dec.Name);

        protected override bool Visit(Phi phi) => phi.AssignedNames.All(_constants.ContainsKey);

        protected override bool Visit(LessThanEqualTo eq) => VisitBinary(eq);

        protected override bool Visit(LessThan eq) => VisitBinary(eq);

        protected override bool Visit(GreaterThanEqualTo eq) => VisitBinary(eq);

        protected override bool Visit(GreaterThan eq) => VisitBinary(eq);

        protected override bool Visit(NotEqualTo eq) => VisitBinary(eq);

        protected override bool Visit(EqualTo eq) => VisitBinary(eq);

        protected override bool Visit(Variable var) => !var.Name.IsExternal && _constants.ContainsKey(var.Name);

        protected override bool Visit(Modulo mod) => VisitBinary(mod);

        protected override bool Visit(PreDecrement dec)
        {
            throw new NotSupportedException();
        }

        protected override bool Visit(PostDecrement dec)
        {
            throw new NotSupportedException();
        }

        protected override bool Visit(PreIncrement inc)
        {
            throw new NotSupportedException();
        }

        protected override bool Visit(PostIncrement inc)
        {
            throw new NotSupportedException();
        }

        protected override bool Visit(Abs app) => Visit(app.Parameter);

        protected override bool Visit(Sqrt app) => Visit(app.Parameter);

        protected override bool Visit(Sine app) => Visit(app.Parameter);

        protected override bool Visit(Cosine app) => Visit(app.Parameter);

        protected override bool Visit(Tangent app) => Visit(app.Parameter);

        protected override bool Visit(ArcSine app) => Visit(app.Parameter);

        protected override bool Visit(ArcCos app) => Visit(app.Parameter);

        protected override bool Visit(ArcTan app) => Visit(app.Parameter);

        protected override bool Visit(Bracketed brk) => Visit(brk.Parameter);

        protected override bool Visit(Add add) => VisitBinary(add);

        protected override bool Visit(Subtract sub) => VisitBinary(sub);

        protected override bool Visit(Multiply mul) => VisitBinary(mul);

        protected override bool Visit(Divide div) => VisitBinary(div);

        protected override bool Visit(Exponent exp) => VisitBinary(exp);

        protected override bool Visit(Negate neg) => Visit(neg.Parameter);

        protected override bool Visit(Factorial fac) => Visit(fac.Parameter);

        protected override bool Visit(ConstantNumber con) => true;

        protected override bool Visit(ConstantString con) => true;
    }
}

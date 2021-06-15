
using System;
using System.Collections.Generic;
using System.Linq;
using Yolol.Analysis.ControlFlowGraph.AST;
using Yolol.Analysis.ControlFlowGraph.Extensions;
using Yolol.Analysis.Types;
using Yolol.Execution;
using Yolol.Execution.Extensions;
using Yolol.Grammar;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Expressions.Binary;
using Yolol.Grammar.AST.Expressions.Unary;
using Yolol.Grammar.AST.Statements;
using Variable = Yolol.Grammar.AST.Expressions.Variable;

namespace Yolol.Analysis.TreeVisitor.Inspection
{
    public class FindBooleanVariables
        : BaseTreeVisitor
    {
        private readonly HashSet<VariableName> _names;
        public IReadOnlyCollection<VariableName> Names => _names;

        // ReSharper disable once NotAccessedField.Local (this field is included in the constructor as a hint that SSA form is required)
        private readonly ISingleStaticAssignmentTable _ssa;
        private readonly ITypeAssignments _types;

        public FindBooleanVariables(HashSet<VariableName> names, ISingleStaticAssignmentTable ssa, ITypeAssignments types)
        {
            _names = names;
            _ssa = ssa;
            _types = types;
        }

        private bool IsBoolean(BaseExpression expr)
        {
            if (expr.IsBoolean)
                return true;

            if (expr is Variable variable)
                return _names.Contains(variable.Name);

            if (expr is Phi phi)
                return phi.AssignedNames.All(_names.Contains);

            return new BooleanExpressionInspector(_names, _types).Visit(expr);
        }

        protected override BaseStatement Visit(TypedAssignment ass)
        {
            if (IsBoolean(ass.Right))
                _names.Add(ass.Left);

            return base.Visit(ass);
        }

        protected override BaseStatement Visit(Assignment ass)
        {
            if (IsBoolean(ass.Right))
                _names.Add(ass.Left);

            return base.Visit(ass);
        }

        private class BooleanExpressionInspector
            : BaseExpressionVisitor<bool>
        {
            private readonly HashSet<VariableName> _bools;

            // ReSharper disable once NotAccessedField.Local
#pragma warning disable IDE0052 // Remove unread private members
            private readonly ITypeAssignments _types;
#pragma warning restore IDE0052 // Remove unread private members

            public BooleanExpressionInspector(HashSet<VariableName> bools, ITypeAssignments types)
            {
                _bools = bools;
                _types = types;
            }

            /// <summary>
            /// Check if an expression that isn't naturally a boolean is in fact a boolean just by chance due to the values used (e.g. `2-1` is a boolean, because it's statically `1`)
            /// </summary>
            /// <param name="expr"></param>
            /// <returns></returns>
            private static bool StaticFalse(BaseExpression expr)
            {
                if (expr.IsBoolean)
                    return true;

                var val = expr.TryStaticEvaluate();

                if (!val.HasValue)
                    return false;

                if (val.Value.Type != Execution.Type.Number)
                    return false;

                return (val.Value.Number == Number.Zero) || (val.Value.Number == Number.One);
            }

            protected override bool Visit(Or or) => true;

            protected override bool Visit(And and) => true;

            protected override bool Visit(Not not) => true;

            protected override bool Visit(Factorial fac) => false;

            protected override bool Visit(ErrorExpression err) => false;

            protected override bool Visit(Increment inc) => false;

            protected override bool Visit(Decrement dec) => false;

            protected override bool Visit(Phi phi) => phi.AssignedNames.All(_bools.Contains);

            protected override bool Visit(LessThanEqualTo eq) => true;

            protected override bool Visit(LessThan eq) => true;

            protected override bool Visit(GreaterThanEqualTo eq) => true;

            protected override bool Visit(GreaterThan eq) => true;

            protected override bool Visit(NotEqualTo eq) => true;

            protected override bool Visit(EqualTo eq) => true;

            protected override bool Visit(Variable var) => _bools.Contains(var.Name);

            // Potentially `A % B` could be considered a boolean iff `A` is a number and `B` is `0` or `1`
            protected override bool Visit(Modulo mod) => StaticFalse(mod);

            protected override bool Visit(PreDecrement dec) => throw new NotSupportedException();

            protected override bool Visit(PostDecrement dec) => throw new NotSupportedException();

            protected override bool Visit(PreIncrement inc) => throw new NotSupportedException();

            protected override bool Visit(PostIncrement inc) => throw new NotSupportedException();

            // `ABS(A)` where A is a boolean always results in a boolean
            protected override bool Visit(Abs app) => StaticFalse(app) || Visit(app.Parameter);

            // `SQRT(A)` where A is a boolean always results in a boolean
            protected override bool Visit(Sqrt app) => StaticFalse(app) || Visit(app.Parameter);

            protected override bool Visit(Sine app) => StaticFalse(app);

            protected override bool Visit(Cosine app) => StaticFalse(app);

            protected override bool Visit(Tangent app) => StaticFalse(app);

            protected override bool Visit(ArcSine app) => StaticFalse(app);

            protected override bool Visit(ArcCos app) => StaticFalse(app);

            protected override bool Visit(ArcTan app) => StaticFalse(app);

            protected override bool Visit(Bracketed brk) => Visit(brk.Parameter);

            protected override bool Visit(Add add) => StaticFalse(add);

            protected override bool Visit(Subtract sub) => StaticFalse(sub);

            // `A * B` where A and B are booleans always results in a boolean
            protected override bool Visit(Multiply mul) => StaticFalse(mul) || (Visit(mul.Left) && Visit(mul.Right));

            protected override bool Visit(Divide div) => StaticFalse(div);

            protected override bool Visit(Exponent exp) => StaticFalse(exp);

            protected override bool Visit(Negate neg) => StaticFalse(neg);

            protected override bool Visit(ConstantNumber con) => StaticFalse(con);

            protected override bool Visit(ConstantString con) => false;
        }
    }
}

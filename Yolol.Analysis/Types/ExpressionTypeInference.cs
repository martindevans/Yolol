using System.Linq;
using JetBrains.Annotations;
using Yolol.Analysis.ControlFlowGraph.AST;
using Yolol.Analysis.TreeVisitor;
using Yolol.Execution;
using Yolol.Execution.Extensions;
using Yolol.Grammar;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Expressions.Binary;
using Yolol.Grammar.AST.Expressions.Unary;

using Type = Yolol.Execution.Type;
using Variable = Yolol.Grammar.AST.Expressions.Variable;

namespace Yolol.Analysis.Types
{
    public class ExpressionTypeInference
        : BaseExpressionVisitor<Type>
    {
        private readonly ITypeAssignments _types;

        public ExpressionTypeInference(ITypeAssignments types)
        {
            _types = types;
        }

        private Type BinaryLogical([NotNull] BaseBinaryExpression expr)
        {
            var l = Visit(expr.Left);
            var r = Visit(expr.Right);

            // If either side is guaranteed to produce an error, so is this
            if (l == Type.Error || r == Type.Error)
                return Type.Error;

            // If a type isn't decided yet we can't type this either
            if (l == Type.Unassigned || r == Type.Unassigned)
                return Type.Unassigned;

            // If either side may produce an error, so may this
            if (l.HasFlag(Type.Error) || r.HasFlag(Type.Error))
                return Type.Error | Type.Number;

            return Type.Number;
        }

        private Type BinaryNumeric([NotNull] BaseBinaryExpression expr, bool forceError)
        {
            var l = Visit(expr.Left);
            var r = Visit(expr.Right);

            // If either side is a guaranteed error, so is this
            if (l == Type.Error || r == Type.Error)
                return Type.Error;

            // If a type isn't decided yet we can't type this either
            if (l == Type.Unassigned || r == Type.Unassigned)
                return Type.Unassigned;

            // If either side is guaranteed not to be a number this is a guaranteed error
            if (!l.HasFlag(Type.Number) || !r.HasFlag(Type.Number))
                return Type.Error;

            // If either side may be a string or an error then this may error
            var mayError = forceError;
            mayError |= l.HasFlag(Type.String);
            mayError |= r.HasFlag(Type.String);
            mayError |= l.HasFlag(Type.Error);
            mayError |= r.HasFlag(Type.Error);
            var err = (mayError ? Type.Error : Type.Unassigned);

            return Type.Number | err;
        }

        private Type BinaryDual([NotNull] BaseBinaryExpression expr)
        {
            var l = Visit(expr.Left);
            var r = Visit(expr.Right);

            // If a type isn't decided yet we can't type this either
            if (l == Type.Unassigned || r == Type.Unassigned)
                return Type.Unassigned;

            // if either side is a guaranteed error, so is this
            if (l == Type.Error || r == Type.Error)
                return Type.Error;

            // If either side may produce an error, so may this
            var err = l.HasFlag(Type.Error) || r.HasFlag(Type.Error) ? Type.Error : Type.Unassigned;

            // Check if either side can be a number
            var ln = l.HasFlag(Type.Number);
            var rn = r.HasFlag(Type.Number);
            var n = ln && rn ? Type.Number : Type.Unassigned;

            // Check if either side can be a string
            var ls = l.HasFlag(Type.String);
            var rs = l.HasFlag(Type.String);
            var s = ls || rs ? Type.String : Type.Unassigned;

            return err | n | s;
        }

#pragma warning disable IDE0060 // Remove unused parameter
        private Type UnaryNumeric([NotNull] BaseExpression expr, [NotNull] BaseExpression parameter, bool forceError, bool allowString)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            var t = base.Visit(parameter);

            // Replace strings with errors if strings are not allowed
            if (t.HasFlag(Type.String) && !allowString)
            {
                t &= ~Type.String;
                t |= Type.Error;
            }

            return t;
        }

        private Type VariableRead([NotNull] VariableName name)
        {
            var t = _types.TypeOf(name);
            if (t.HasValue)
                return t.Value;

            if (name.IsExternal)
                return Type.String | Type.Number;
            else
                return _types.TypeOf(name) ?? Type.Unassigned;
        }

        protected override Type Visit(Increment inc)
        {
            return _types.TypeOf(inc.Name) ?? Type.Unassigned;
        }

        protected override Type Visit(Decrement dec)
        {
            return _types.TypeOf(dec.Name) ?? Type.Unassigned;
        }

        protected override Type Visit(ErrorExpression err)
        {
            return Type.Error;
        }

        protected override Type Visit(Phi phi)
        {
            var result = Type.Unassigned;
            foreach (var type in phi.AssignedNames.Select(_types.TypeOf))
            {
                if (type.HasValue)
                    result |= type.Value;
            }

            return result;
        }

        protected override Type Visit(And and) => BinaryLogical(and);

        protected override Type Visit(Or or) => BinaryLogical(or);

        protected override Type Visit([NotNull] Not not) => Type.Number;

        protected override Type Visit(LessThanEqualTo eq) => BinaryLogical(eq);

        protected override Type Visit(LessThan eq) => BinaryLogical(eq);

        protected override Type Visit(GreaterThanEqualTo eq) => BinaryLogical(eq);

        protected override Type Visit(GreaterThan eq) => BinaryLogical(eq);

        protected override Type Visit(NotEqualTo eq) => BinaryLogical(eq);

        protected override Type Visit(EqualTo eq) => BinaryLogical(eq);

        protected override Type Visit(Variable var) => VariableRead(var.Name);

        protected override Type Visit(Modulo mod) => BinaryNumeric(mod, true);

        protected override Type Visit(PreDecrement dec) => VariableRead(dec.Name);

        protected override Type Visit(PostDecrement dec) => VariableRead(dec.Name);

        protected override Type Visit(PreIncrement inc) => VariableRead(inc.Name);

        protected override Type Visit(PostIncrement inc) => VariableRead(inc.Name);

        protected override Type Visit(Abs app) => UnaryNumeric(app, app.Parameter, false, false);

        protected override Type Visit(Sqrt app) => UnaryNumeric(app, app.Parameter, true, false);

        protected override Type Visit(Sine app) => UnaryNumeric(app, app.Parameter, false, false);

        protected override Type Visit(Cosine app) => UnaryNumeric(app, app.Parameter, false, false);

        protected override Type Visit(Tangent app) => UnaryNumeric(app, app.Parameter, true, false);

        protected override Type Visit(ArcSine app) => UnaryNumeric(app, app.Parameter, true, false);

        protected override Type Visit(ArcCos app) => UnaryNumeric(app, app.Parameter, true, false);

        protected override Type Visit(ArcTan app) => UnaryNumeric(app, app.Parameter, true, false);

        protected override Type Visit(Bracketed brk) => Visit(brk.Parameter);

        protected override Type Visit(Add add) => BinaryDual(add);

        protected override Type Visit(Subtract sub) => BinaryDual(sub);

        protected override Type Visit(Multiply mul) => BinaryNumeric(mul, false);

        protected override Type Visit(Divide div)
        {
            // If we statically know the right side is a non-zero number don't force an error
            var forceError = true;
            if (div.Right.IsConstant)
            {
                var r = div.Right.StaticEvaluate();
                if (r.Type == Type.Number && r.Number != 0)
                    forceError = false;
            }

            return BinaryNumeric(div, forceError);
        }

        protected override Type Visit(Exponent exp) => BinaryNumeric(exp, true);

        protected override Type Visit(Negate neg) => UnaryNumeric(neg, neg.Parameter, false, false);

        protected override Type Visit(ConstantNumber con) => Type.Number;

        protected override Type Visit(ConstantString con) => Type.String;
    }
}
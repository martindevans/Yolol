using System;
using Yolol.Analysis.ControlFlowGraph.AST;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Expressions.Binary;
using Yolol.Grammar.AST.Expressions.Unary;

namespace Yolol.Analysis.TreeVisitor
{
    public abstract class BaseExpressionVisitor<TResult>
    {
        #region expression visiting
        public virtual TResult Visit(BaseExpression expression)
        {
            return expression switch {
                Phi a => Visit(a),
                Increment a => Visit(a),
                Decrement a => Visit(a),
                ErrorExpression a => Visit(a),
                Bracketed a => Visit(a),
                Abs a => Visit(a),
                Sqrt a => Visit(a),
                Sine a => Visit(a),
                Cosine a => Visit(a),
                Tangent a => Visit(a),
                ArcSine a => Visit(a),
                ArcCos a => Visit(a),
                ArcTan a => Visit(a),
                PostIncrement a => Visit(a),
                PreIncrement a => Visit(a),
                PostDecrement a => Visit(a),
                PreDecrement a => Visit(a),
                Add a => Visit(a),
                Subtract a => Visit(a),
                Multiply a => Visit(a),
                Divide a => Visit(a),
                Modulo a => Visit(a),
                Negate a => Visit(a),
                Exponent a => Visit(a),
                Or a => Visit(a),
                And a => Visit(a),
                Not a => Visit(a),
                Factorial a => Visit(a),
                Variable a => Visit(a),
                ConstantNumber a => Visit(a),
                ConstantString a => Visit(a),
                EqualTo a => Visit(a),
                NotEqualTo a => Visit(a),
                GreaterThan a => Visit(a),
                GreaterThanEqualTo a => Visit(a),
                LessThan a => Visit(a),
                LessThanEqualTo a => Visit(a),
                _ => VisitUnknown(expression)
            };
        }

        protected virtual TResult VisitUnknown(BaseExpression expression)
        {
            throw new InvalidOperationException($"`Visit` not valid for expression type `{expression.GetType().FullName}`");
        }

        protected abstract TResult Visit(Or or);

        protected abstract TResult Visit(And and);

        protected abstract TResult Visit(Not not);

        protected abstract TResult Visit(Factorial fac);

        protected abstract TResult Visit(ErrorExpression err);

        protected abstract TResult Visit(Increment inc);

        protected abstract TResult Visit(Decrement dec);

        protected abstract TResult Visit(Phi phi);

        protected abstract TResult Visit(LessThanEqualTo eq);

        protected abstract TResult Visit(LessThan eq);

        protected abstract TResult Visit(GreaterThanEqualTo eq);

        protected abstract TResult Visit(GreaterThan eq);

        protected abstract TResult Visit(NotEqualTo eq);

        protected abstract TResult Visit(EqualTo eq);

        protected abstract TResult Visit(Variable var);

        protected abstract TResult Visit(Modulo mod);

        protected abstract TResult Visit(PreDecrement dec);

        protected abstract TResult Visit(PostDecrement dec);

        protected abstract TResult Visit(PreIncrement inc);

        protected abstract TResult Visit(PostIncrement inc);

        protected abstract TResult Visit(Abs app);

        protected abstract TResult Visit(Sqrt app);

        protected abstract TResult Visit(Sine app);

        protected abstract TResult Visit(Cosine app);

        protected abstract TResult Visit(Tangent app);

        protected abstract TResult Visit(ArcSine app);

        protected abstract TResult Visit(ArcCos app);

        protected abstract TResult Visit(ArcTan app);

        protected abstract TResult Visit(Bracketed brk);

        protected abstract TResult Visit(Add add);

        protected abstract TResult Visit(Subtract sub);

        protected abstract TResult Visit(Multiply mul);

        protected abstract TResult Visit(Divide div);

        protected abstract TResult Visit(Exponent exp);

        protected abstract TResult Visit(Negate neg);

        protected abstract TResult Visit(ConstantNumber con);

        protected abstract TResult Visit(ConstantString con);
        #endregion
    }
}

using System;
using JetBrains.Annotations;
using Yolol.Analysis.ControlFlowGraph.AST;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Expressions.Binary;
using Yolol.Grammar.AST.Expressions.Unary;

namespace Yolol.Analysis.TreeVisitor
{
    public abstract class BaseExpressionVisitor<TResult>
    {
        #region expression visiting
        [NotNull] public virtual TResult Visit([NotNull] BaseExpression expression)
        {
            switch (expression)
            {
                case Phi a:       return Visit(a);
                case Increment a: return Visit(a);
                case Decrement a: return Visit(a);
                case ErrorExpression a: return Visit(a);

                case Bracketed a:   return Visit(a);
                
                case Abs a:     return Visit(a);
                case Sqrt a:    return Visit(a);
                case Sine a:    return Visit(a);
                case Cosine a:  return Visit(a);
                case Tangent a: return Visit(a);
                case ArcSine a: return Visit(a);
                case ArcCos a:  return Visit(a);
                case ArcTan a:  return Visit(a);

                case PostIncrement a: return Visit(a);
                case PreIncrement a:  return Visit(a);
                case PostDecrement a: return Visit(a);
                case PreDecrement a:  return Visit(a);

                case Add a:      return Visit(a);
                case Subtract a: return Visit(a);
                case Multiply a: return Visit(a);
                case Divide a:   return Visit(a);
                case Modulo a:   return Visit(a);
                case Negate a:   return Visit(a);
                case Exponent a: return Visit(a);

                case Or a:  return Visit(a);
                case And a: return Visit(a);
                case Not a: return Visit(a);

                case Variable a:       return Visit(a);
                case ConstantNumber a: return Visit(a);
                case ConstantString a: return Visit(a);

                case EqualTo a:            return Visit(a);
                case NotEqualTo a:         return Visit(a);
                case GreaterThan a:        return Visit(a);
                case GreaterThanEqualTo a: return Visit(a);
                case LessThan a:           return Visit(a);
                case LessThanEqualTo a:    return Visit(a);
            }

            return VisitUnknown(expression);
        }

        [NotNull] protected virtual TResult VisitUnknown(BaseExpression expression)
        {
            throw new InvalidOperationException($"`Visit` not invalid for expression type `{expression.GetType().FullName}`");
        }

        [NotNull] protected abstract TResult Visit([NotNull] Or or);

        [NotNull] protected abstract TResult Visit([NotNull] And and);

        [NotNull] protected abstract TResult Visit([NotNull] Not not);

        [NotNull] protected abstract TResult Visit([NotNull] ErrorExpression err);

        [NotNull] protected abstract TResult Visit([NotNull] Increment inc);

        [NotNull] protected abstract TResult Visit([NotNull] Decrement dec);

        [NotNull] protected abstract TResult Visit([NotNull] Phi phi);

        [NotNull] protected abstract TResult Visit([NotNull] LessThanEqualTo eq);

        [NotNull] protected abstract TResult Visit([NotNull] LessThan eq);

        [NotNull] protected abstract TResult Visit([NotNull] GreaterThanEqualTo eq);

        [NotNull] protected abstract TResult Visit([NotNull] GreaterThan eq);

        [NotNull] protected abstract TResult Visit([NotNull] NotEqualTo eq);

        [NotNull] protected abstract TResult Visit([NotNull] EqualTo eq);

        [NotNull] protected abstract TResult Visit([NotNull] Variable var);

        [NotNull] protected abstract TResult Visit([NotNull] Modulo mod);

        [NotNull] protected abstract TResult Visit([NotNull] PreDecrement dec);

        [NotNull] protected abstract TResult Visit([NotNull] PostDecrement dec);

        [NotNull] protected abstract TResult Visit([NotNull] PreIncrement inc);

        [NotNull] protected abstract TResult Visit([NotNull] PostIncrement inc);

        [NotNull] protected abstract TResult Visit([NotNull] Abs app);

        [NotNull] protected abstract TResult Visit([NotNull] Sqrt app);

        [NotNull] protected abstract TResult Visit([NotNull] Sine app);

        [NotNull] protected abstract TResult Visit([NotNull] Cosine app);

        [NotNull] protected abstract TResult Visit([NotNull] Tangent app);

        [NotNull] protected abstract TResult Visit([NotNull] ArcSine app);

        [NotNull] protected abstract TResult Visit([NotNull] ArcCos app);

        [NotNull] protected abstract TResult Visit([NotNull] ArcTan app);

        [NotNull] protected abstract TResult Visit([NotNull] Bracketed brk);

        [NotNull] protected abstract TResult Visit([NotNull] Add add);

        [NotNull] protected abstract TResult Visit([NotNull] Subtract sub);

        [NotNull] protected abstract TResult Visit([NotNull] Multiply mul);

        [NotNull] protected abstract TResult Visit([NotNull] Divide div);

        [NotNull] protected abstract TResult Visit([NotNull] Exponent exp);

        [NotNull] protected abstract TResult Visit([NotNull] Negate neg);

        [NotNull] protected abstract TResult Visit([NotNull] ConstantNumber con);

        [NotNull] protected abstract TResult Visit([NotNull] ConstantString con);
        #endregion
    }
}

using System;
using JetBrains.Annotations;
using Yolol.Execution;
using Type = Yolol.Execution.Type;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class Negate
        : BaseUnaryExpression, IEquatable<Negate>
    {
        public override bool CanRuntimeError => Parameter.CanRuntimeError;

        public override bool IsBoolean => false;

        public Negate([NotNull] BaseExpression parameter) : base(parameter) { }

        protected override Value Evaluate([NotNull] string parameterValue)
        {
            throw new ExecutionException("Attempted to negate a String value");
        }

        protected override Value Evaluate(Number parameterValue)
        {
            return new Value(-parameterValue);
        }

        public bool Equals(Negate other)
        {
            return other != null
                && other.Parameter.Equals(Parameter);
        }

        public override bool Equals(BaseExpression other)
        {
            return other is Negate neg
                && neg.Equals(this);
        }

        public override string ToString()
        {
            return $"-{Parameter}";
        }
    }
}

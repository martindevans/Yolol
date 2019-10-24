using System;
using JetBrains.Annotations;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class Bracketed
        : BaseUnaryExpression, IEquatable<Bracketed>
    {
        public override bool CanRuntimeError => Parameter.CanRuntimeError;

        public Bracketed([NotNull] BaseExpression parameter) : base(parameter) { }

        protected override Value Evaluate([NotNull] string parameterValue)
        {
            return new Value(parameterValue);
        }

        protected override Value Evaluate(Number parameterValue)
        {
            return new Value(parameterValue);
        }
        
        public bool Equals(Bracketed other)
        {
            return other != null
                && other.Parameter.Equals(Parameter);
        }

        public override bool Equals(BaseExpression other)
        {
            return other is Bracketed brk
                && brk.Equals(this);
        }

        public override string ToString()
        {
            return $"({Parameter})";
        }
    }
}

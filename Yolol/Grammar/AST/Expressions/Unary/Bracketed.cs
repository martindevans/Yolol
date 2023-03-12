using System;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class Bracketed
        : BaseUnaryExpression, IEquatable<Bracketed>
    {
        public override bool CanRuntimeError => Parameter.CanRuntimeError;

        public override bool IsBoolean => Parameter.IsBoolean;


        public Bracketed(BaseExpression parameter)
            : base(parameter)
        {
        }

        protected override Value Evaluate(Value value, int maxStringLength) => value;

        public bool Equals(Bracketed? other)
        {
            return other != null
                && other.Parameter.Equals(Parameter);
        }

        public override bool Equals(BaseExpression? other)
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

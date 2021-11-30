using System;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class Not
        : BaseUnaryExpression, IEquatable<Not>
    {
        public override bool CanRuntimeError => Parameter.CanRuntimeError;

        public override bool IsBoolean => true;

        public Not(BaseExpression parameter)
            : base(parameter)
        {
        }

        protected override Value Evaluate(Value value) => new Value(!value);

        public bool Equals(Not? other)
        {
            return other != null
                && other.Parameter.Equals(Parameter);
        }

        public override bool Equals(BaseExpression? other)
        {
            return other is Not not
                && not.Equals(this);
        }

        public override string ToString()
        {
            return $"not {Parameter}";
        }
    }
}

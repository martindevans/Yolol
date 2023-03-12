using System;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class Tangent
        : BaseTrigonometry, IEquatable<Tangent>
    {
        public Tangent(BaseExpression parameter)
            : base(parameter, "tan")
        {
        }

        protected override Value Evaluate(Value value, int maxStringLength)
        {
            return Value.Tan(value);
        }

        public bool Equals(Tangent? other)
        {
            return other != null 
                && other.Parameter.Equals(Parameter);
        }

        public override bool Equals(BaseExpression? other)
        {
            return other is Tangent tan
                && tan.Equals(this);
        }
    }
}

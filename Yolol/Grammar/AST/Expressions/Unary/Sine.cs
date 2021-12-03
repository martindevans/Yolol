using System;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class Sine
        : BaseTrigonometry, IEquatable<Sine>
    {
        public Sine(BaseExpression parameter)
            : base(parameter, "sin")
        {
        }

        protected override Value Evaluate(Value value, int _)
        {
            return Value.Sin(value);
        }

        public bool Equals(Sine? other)
        {
            return other != null 
                && other.Parameter.Equals(Parameter);
        }

        public override bool Equals(BaseExpression? other)
        {
            return other is Sine sine
                && sine.Equals(this);
        }
    }
}

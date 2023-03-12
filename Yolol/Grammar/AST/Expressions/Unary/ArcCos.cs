using System;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class ArcCos
        : BaseTrigonometry, IEquatable<ArcCos>
    {
        public ArcCos(BaseExpression parameter)
            : base(parameter, "acos")
        {
        }

        protected override Value Evaluate(Value value, int maxStringLength)
        {
            return Value.ArcCos(value);
        }

        public bool Equals(ArcCos? other)
        {
            return other != null 
                && other.Parameter.Equals(Parameter);
        }

        public override bool Equals(BaseExpression? other)
        {
            return other is ArcCos acos
                && acos.Equals(this);
        }
    }
}

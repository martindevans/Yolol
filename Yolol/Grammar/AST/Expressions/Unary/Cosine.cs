using System;
using JetBrains.Annotations;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class Cosine
        : BaseTrigonometry, IEquatable<Cosine>
    {
        public Cosine([NotNull] BaseExpression parameter)
            : base(parameter, "cos")
        {
        }

        protected override Value Evaluate(Value value)
        {
            return Value.Cos(value);
        }

        public bool Equals(Cosine other)
        {
            return other != null 
                && other.Parameter.Equals(Parameter);
        }

        public override bool Equals(BaseExpression other)
        {
            return other is Cosine cos
                && cos.Equals(this);
        }
    }
}

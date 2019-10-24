using System;
using JetBrains.Annotations;
using Yolol.Execution;
using Type = Yolol.Execution.Type;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class Not
        : BaseUnaryExpression, IEquatable<Not>
    {
        public override bool CanRuntimeError => Parameter.CanRuntimeError;

        public override bool IsBoolean => true;

        public Not([NotNull] BaseExpression parameter): base(parameter) { }

        protected override Value Evaluate([NotNull] string parameterValue)
        {
            return new Value(false);
        }

        protected override Value Evaluate(Number parameterValue)
        {
            return new Value(parameterValue.Value == 0);
        }

        public bool Equals(Not other)
        {
            return other != null
                && other.Parameter.Equals(Parameter);
        }

        public override bool Equals(BaseExpression other)
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

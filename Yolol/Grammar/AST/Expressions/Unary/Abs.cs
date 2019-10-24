using System;
using JetBrains.Annotations;
using Yolol.Execution;
using Type = Yolol.Execution.Type;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class Abs
        : BaseUnaryExpression, IEquatable<Abs>
    {
        public override bool CanRuntimeError => true;

        public override bool IsBoolean => false;

        public override bool IsConstant => Parameter.IsConstant;

        public Abs([NotNull] BaseExpression parameter):base(parameter) { }

        protected override Value Evaluate([NotNull] string parameterValue)
        {
            throw new ExecutionException("Attempted to Abs a string value");
        }

        protected override Value Evaluate(Number parameterValue)
        {
            if (parameterValue < 0)
                return new Value(-parameterValue);
            else
                return new Value(parameterValue);
        }

        public bool Equals(Abs other)
        {
            return other != null 
                && other.Parameter.Equals(Parameter);
        }

        public override bool Equals(BaseExpression other)
        {
            return other is Abs abs
                && abs.Equals(this);
        }

        public override string ToString()
        {
            return $"ABS({Parameter})";
        }
    }
}

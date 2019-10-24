using System;
using JetBrains.Annotations;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public abstract class BaseTrigonometry
        : BaseUnaryExpression
    {
        private readonly string _name;
        private readonly bool _convertInputToRadians;
        private readonly bool _convertOutputToDegrees;

        public override bool CanRuntimeError => true;

        public override bool IsBoolean => false;

        protected BaseTrigonometry([NotNull] BaseExpression parameter, string name, bool convertInputToRadians, bool convertOutputToDegrees): base(parameter)
        {
            _name = name;
            _convertInputToRadians = convertInputToRadians;
            _convertOutputToDegrees = convertOutputToDegrees;
        }

        protected override Value Evaluate([NotNull] string parameterValue)
        {
            throw new ExecutionException($"Attempted to {_name} a string value");
        }

        protected override Value Evaluate(Number parameterValue)
        {
            var converted = _convertInputToRadians ? Radians(parameterValue.Value) : (double)parameterValue.Value;
            var result = Evaluate(converted);
            return new Value(new Number(_convertOutputToDegrees ? Degrees(result) : (decimal)result));
        }

        private static decimal Degrees(double radians)
        {
            return (decimal)(radians * (180.0 / Math.PI));
        }

        private static double Radians(decimal degrees)
        {
            return Math.PI * (double)degrees / 180.0;
        }

        protected abstract double Evaluate(double value);

        public override string ToString()
        {
            return $"{_name}({Parameter})";
        }
    }
}

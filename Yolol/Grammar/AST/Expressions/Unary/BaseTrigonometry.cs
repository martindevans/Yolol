using System;
using JetBrains.Annotations;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public abstract class BaseTrigonometry
        : BaseExpression
    {
        private readonly string _name;
        private readonly bool _convertInputToRadians;
        private readonly bool _convertOutputToDegrees;
        [NotNull] public BaseExpression Parameter { get; }

        public override bool CanRuntimeError => true;

        public override bool IsBoolean => false;

        public override bool IsConstant => Parameter.IsConstant;

        protected BaseTrigonometry([NotNull] BaseExpression parameter, string name, bool convertInputToRadians, bool convertOutputToDegrees)
        {
            _name = name;
            _convertInputToRadians = convertInputToRadians;
            _convertOutputToDegrees = convertOutputToDegrees;

            Parameter = parameter;
        }

        public override Value Evaluate(MachineState state)
        {
            var input = Parameter.Evaluate(state);

            if (input.Type == Execution.Type.String)
                throw new ExecutionException($"Attempted to {_name} a string value");

            var converted = _convertInputToRadians ? Radians(input.Number.Value) : (double)input.Number.Value;
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

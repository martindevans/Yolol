using System;
using Yolol.Grammar.AST.Expressions;

namespace Yolol.Execution.Extensions
{
    public static class BaseExpressionExtensions
    {
        public static Value StaticEvaluate(this BaseExpression expression)
        {
            if (!expression.IsConstant)
                throw new ArgumentException("Cannot statically evaluate a non-constant expression");

            return expression.Evaluate(new MachineState(new NullDeviceNetwork()));
        }

        public static Value? TryStaticEvaluate(this BaseExpression expression)
        {
            return expression.TryStaticEvaluate(out _);
        }

        public static Value? TryStaticEvaluate(this BaseExpression expression, out bool runtimeError)
        {
            if (!expression.IsConstant)
            {
                runtimeError = false;
                return null;
            }

            try
            {
                runtimeError = false;
                return expression.Evaluate(new MachineState(new ThrowDeviceNetwork()));
            }
            catch (ThrowDeviceNetwork.NullDeviceNetworkAccessException)
            {
                // Attempted to access an external field, we can't statically evaluate that
                runtimeError = false;
                return null;
            }
            catch (ExecutionException)
            {
                // Some other execution exception happened
                runtimeError = true;
                return null;
            }
        }
    }
}

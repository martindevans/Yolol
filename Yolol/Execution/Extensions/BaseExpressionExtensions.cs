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
            try
            {
                return expression.Evaluate(new MachineState(new ThrowDeviceNetwork()));
            }
            catch (ThrowDeviceNetwork.NullDeviceNetworkAccessException)
            {
                // Attempted to access an external field, we can't statically evaluate that
                return null;
            }
            catch (ExecutionException)
            {
                // Some other execution exception happened
                return null;
            }
        }
    }
}

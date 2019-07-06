using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Yolol.Grammar.AST.Expressions;

namespace Yolol.Execution.Extensions
{
    public static class BaseExpressionExtensions
    {
        public static Value StaticEvaluate([NotNull] this BaseExpression expression)
        {
            if (!expression.IsConstant)
                throw new ArgumentException("Cannot statically evaluate a non-constant expression");

            return expression.Evaluate(new MachineState(new NullDeviceNetwork(), new DefaultIntrinsics()));
        }
    }
}

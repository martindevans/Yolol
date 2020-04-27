﻿using System;
using System.Collections.Generic;
using System.Text;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Unary
{
    public class Factorial
        : BaseUnaryExpression
    {
        public override bool IsBoolean => false;

        public override bool CanRuntimeError => true;

        public Factorial(BaseExpression parameter)
            : base(parameter)
        {
        }

        
        public bool Equals(Factorial? other)
        {
            return other != null 
                && other.Parameter.Equals(Parameter);
        }

        public override bool Equals(BaseExpression? other)
        {
            return other is Factorial fac
                && fac.Equals(this);
        }

        public override string ToString()
        {
            return $"{Parameter}!";
        }

        protected override Value Evaluate(Value value)
        {
            if (value.Type == Execution.Type.String)
                throw new ExecutionException("Attempted to apply factorial to a string");
            else
                throw new NotImplementedException();
        }
    }
}
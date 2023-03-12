﻿using System;
using Yolol.Execution;

namespace Yolol.Grammar.AST.Expressions.Binary
{
    public class Divide
        : BaseBinaryExpression, IEquatable<Divide>
    {
        public override bool CanRuntimeError => true;

        public Divide(BaseExpression left, BaseExpression right)
            : base(left, right)
        {
        }

        protected override Value Evaluate(Value l, Value r, int maxStringLength)
        {
            return l / r;
        }

        public bool Equals(Divide? other)
        {
            return other != null
                   && other.Left.Equals(Left)
                   && other.Right.Equals(Right);
        }

        public override bool Equals(BaseExpression? other)
        {
            return other is Divide a
                && a.Equals(this);
        }

        public override string ToString()
        {
            return $"{Left}/{Right}";
        }
    }
}

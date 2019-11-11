using System;
using System.Globalization;
using JetBrains.Annotations;
using Yolol.Grammar.AST.Expressions;

namespace Yolol.Execution
{
    public struct Value
        : IEquatable<Value>
    {
        public Type Type { get; }

        private readonly Number _number;
        public Number Number
        {
            get
            {
                if (Type != Type.Number)
                    throw new InvalidCastException($"Attempted to access value of type {Type} as a Number");
                return _number;
            }
        }

        private readonly string _string;
        public string String
        {
            get
            {
                if (Type != Type.String)
                    throw new InvalidCastException($"Attempted to access value of type {Type} as a String");
                return _string;
            }
        }

        public Value(string str)
        {
            _string = str;
            _number = new Number(0);
            Type = Type.String;
        }

        public Value(Number num)
        {
            _string = "";
            _number = num;
            Type = Type.Number;
        }

        public Value(bool @bool)
            : this(@bool ? 1 : 0)
        {
        }

        public static implicit operator Value(Number n)
        {
            return new Value(n);
        }

        public static implicit operator Value(decimal d)
        {
            return new Value(new Number(d));
        }

        public static implicit operator Value(string s)
        {
            return new Value(s);
        }

        public override string ToString()
        {
            if (Type == Type.Number)
                return Number.ToString(CultureInfo.InvariantCulture);

            return $"\"{String}\"";
        }

        public bool ToBool()
        {
            if (Type == Type.String)
                return true;
            else
                return Number != 0;
        }

        public object ToObject()
        {
            if (Type == Type.Number)
                return Number.Value;
            else
                return String;
        }

        public bool Equals(Value other)
        {
            if (Type != other.Type)
                return false;

            if (Type == Type.String)
                return _string.Equals(other._string);

            // ReSharper disable once ImpureMethodCallOnReadonlyValueField
            return _number.Equals(other._number);
            
        }

        public override bool Equals(object obj)
        {
            return obj is Value other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)Type * 397;

                if (Type == Type.String)
                    hashCode += _string?.GetHashCode() ?? 1;
                else
                    hashCode += _number.GetHashCode();

                return hashCode;
            }
        }

        public static bool operator ==(Value left, Value right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Value left, Value right)
        {
            return !left.Equals(right);
        }

        [NotNull] public BaseExpression ToConstant()
        {
            if (Type == Type.Number)
                return new ConstantNumber(Number);
            else
                return new ConstantString(String);
        }
    }
}

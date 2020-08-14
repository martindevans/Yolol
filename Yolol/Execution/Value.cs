using System;
using Yolol.Grammar.AST.Expressions;

namespace Yolol.Execution
{
    public readonly struct Value
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

        private readonly YString _string;
        public YString String
        {
            get
            {
                if (Type != Type.String)
                    throw new InvalidCastException($"Attempted to access value of type {Type} as a String");
                return _string;
            }
        }

        public Value(ReadOnlyMemory<char> str)
        {
            _string = new YString(str);
            _number = Number.Zero;
            Type = Type.String;
        }

        public Value(string str)
        {
            _string = new YString(str);
            _number = Number.Zero;
            Type = Type.String;
        }

        public Value(YString str)
        {
            _string = str;
            _number = Number.Zero;
            Type = Type.String;
        }

        public Value(Number num)
        {
            _string = default;
            _number = num;
            Type = Type.Number;
        }

        public Value(bool @bool)
            : this(@bool ? Number.One : Number.Zero)
        {
        }

        public static implicit operator Value(Number n)
        {
            return new Value(n);
        }

        public static implicit operator Value(decimal d)
        {
            return new Value(d);
        }

        public static implicit operator Value(string s)
        {
            return new Value(s);
        }

        public override string ToString()
        {
            if (Type == Type.Number)
                return _number.ToString();
            else
                return _string.ToString();
        }

        private YString ToYString()
        {
            if (Type == Type.Number)
                return new YString(_number.ToString());
            else
                return _string;
        }

        public bool ToBool()
        {
            if (Type == Type.String)
                return true;
            else
                return _number != 0;
        }

        public object ToObject()
        {
            if (Type == Type.Number)
                return (decimal)_number;
            else
                return String.ToString();
        }

        public bool Equals(Value other)
        {
            if (Type == Type.Number && other.Type == Type.Number)
                return _number == other._number;
            else if (Type == Type.String && other.Type == Type.String)
                return _string == other._string;
            else
                return false;
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
                    hashCode *= _string.GetHashCode();
                else
                    hashCode *= _number.GetHashCode();

                return hashCode;
            }
        }


        public static bool operator <(Value left, Value right)
        {
            if (left.Type == Type.Number)
                return left._number < right;
            else
                return left._string < right;
        }

        public static bool operator <(Value left, YString right)
        {
            if (left.Type == Type.Number)
                return left._number < right;
            else
                return left._string < right;
        }

        public static bool operator <(Value left, Number right)
        {
            if (left.Type == Type.Number)
                return left._number < right;
            else
                return left._string < right;
        }

        public static bool operator <(Value left, bool right)
        {
            if (left.Type == Type.Number)
                return left._number < right;
            else
                return left._string < right;
        }


        public static bool operator <=(Value left, Value right)
        {
            if (left.Type == Type.Number)
                return left._number <= right;
            else
                return left._string <= right;
        }

        public static bool operator <=(Value left, YString right)
        {
            if (left.Type == Type.Number)
                return left._number <= right;
            else
                return left._string <= right;
        }

        public static bool operator <=(Value left, Number right)
        {
            if (left.Type == Type.Number)
                return left._number <= right;
            else
                return left._string <= right;
        }

        public static bool operator <=(Value left, bool right)
        {
            if (left.Type == Type.Number)
                return left._number <= right;
            else
                return left._string <= right;
        }


        public static bool operator >(Value left, Value right)
        {
            if (left.Type == Type.Number)
                return left._number > right;
            else
                return left._string > right;
        }

        public static bool operator >(Value left, YString right)
        {
            if (left.Type == Type.Number)
                return left._number > right;
            else
                return left._string > right;
        }

        public static bool operator >(Value left, Number right)
        {
            if (left.Type == Type.Number)
                return left._number > right;
            else
                return left._string > right;
        }

        public static bool operator >(Value left, bool right)
        {
            if (left.Type == Type.Number)
                return left._number > right;
            else
                return left._string > right;
        }


        public static bool operator >=(Value left, Value right)
        {
            if (left.Type == Type.Number)
                return left._number >= right;
            else
                return left._string >= right;
        }

        public static bool operator >=(Value left, YString right)
        {
            if (left.Type == Type.Number)
                return left._number >= right;
            else
                return left._string >= right;
        }

        public static bool operator >=(Value left, Number right)
        {
            if (left.Type == Type.Number)
                return left._number >= right;
            else
                return left._string >= right;
        }

        public static bool operator >=(Value left, bool right)
        {
            if (left.Type == Type.Number)
                return left._number >= right;
            else
                return left._string >= right;
        }


        public static bool operator ==(Value left, Value right)
        {
            return left.Equals(right);
        }

        public static bool operator ==(Value left, YString right)
        {
            if (left.Type != Type.String)
                return false;
            else
                return left._string == right;
        }

        public static bool operator ==(Value left, Number right)
        {
            if (left.Type != Type.Number)
                return false;
            else
                return left._number == right;
        }

        public static bool operator ==(Value left, bool right)
        {
            if (left.Type != Type.Number)
                return false;
            else
                return left._number == right;
        }


        public static bool operator !=(Value left, Value right)
        {
            return !left.Equals(right);
        }

        public static bool operator !=(Value left, YString right)
        {
            if (left.Type != Type.String)
                return true;
            else
                return left._string != right;
        }

        public static bool operator !=(Value left, Number right)
        {
            if (left.Type != Type.Number)
                return true;
            else
                return left._number != right;
        }

        public static bool operator !=(Value left, bool right)
        {
            if (left.Type != Type.Number)
                return true;
            else
                return left._number != right;
        }


        public static Value operator -(Value left, Value right)
        {
            if (left.Type == Type.Number && right.Type == Type.Number)
                return left._number - right._number;
            else
                return new Value(left.ToYString() - right.ToYString());
        }

        public static YString operator -(Value left, YString right)
        {
            return left.ToYString() - right;
        }

        public static Value operator -(Value l, Number r)
        {
            if (l.Type == Type.Number)
                return l._number - r;
            else
                return new Value(l._string - r);
        }

        public static Value operator -(Value l, bool r)
        {
            return l - (Number)r;
        }


        public static Value operator +(Value left, Value right)
        {
            if (left.Type == Type.Number && right.Type == Type.Number)
                return left._number + right._number;
            else
                return new Value(left.ToYString() + right.ToYString());
        }

        public static YString operator +(Value left, YString right)
        {
            return left.ToYString() + right;
        }

        public static Value operator +(Value l, Number r)
        {
            if (l.Type == Type.Number)
                return l._number + r;
            else
                return new Value(l._string + r);
        }

        public static Value operator +(Value l, bool r)
        {
            return l + (r ? Number.One : Number.Zero);
        }


        public static Number operator *(Value left, Value right)
        {
            if (left.Type == Type.Number && right.Type == Type.Number)
                return left._number * right._number;
            else
                throw new ExecutionException("Attempted to multiply a string");
        }

        public static StaticError operator *(Value left, YString right)
        {
            throw new ExecutionException("Attempted to multiply a string");
        }

        public static Value operator *(Value left, Number right)
        {
            if (left.Type == Type.Number)
                return left._number * right;
            else
                throw new ExecutionException("Attempted to multiply a string");
        }

        public static Value operator *(Value left, bool right)
        {
            if (left.Type == Type.Number)
                return left._number * right;
            else
                throw new ExecutionException("Attempted to multiply a string");
        }


        public static Number operator /(Value left, Value right)
        {
            if (left.Type == Type.Number && right.Type == Type.Number)
                return left._number / right._number;
            else
                throw new ExecutionException("Attempted to divide a string");
        }

        public static StaticError operator /(Value _, YString __)
        {
            throw new ExecutionException("Attempted to divide a string");
        }

        public static Value operator /(Value left, Number right)
        {
            if (left.Type == Type.Number)
                return left._number / right;
            else
                throw new ExecutionException("Attempted to divide a string");
        }

        public static Value operator /(Value left, bool right)
        {
            if (left.Type == Type.Number)
                return left._number / right;
            else
                throw new ExecutionException("Attempted to divide a string");
        }


        public static bool operator &(Value left, Value right)
        {
            return left.ToBool() & right.ToBool();
        }

        public static bool operator |(Value left, Value right)
        {
            return left.ToBool() | right.ToBool();
        }


        public static Number operator %(Value left, Value right)
        {
            if (left.Type == Type.Number && right.Type == Type.Number)
                return left._number % right._number;
            else
                throw new ExecutionException("Attempted to modulo a string");
        }

        public static Number operator %(Value _, YString __)
        {
            throw new ExecutionException("Attempted to modulo a string");
        }

        public static Number operator %(Value left, Number right)
        {
            if (left.Type == Type.Number)
                return left._number % right;
            else
                throw new ExecutionException("Attempted to modulo a string");
        }

        public static Number operator %(Value left, bool right)
        {
            if (left.Type == Type.Number)
                return left._number % (Number)right;
            else
                throw new ExecutionException("Attempted to modulo a string");
        }


        public static Value operator ++(Value value)
        {
            if (value.Type == Type.Number)
                return new Value(value._number + 1);

            var a = value._string;
            a++;
            return new Value(a);
        }

        public static Value operator --(Value value)
        {
            if (value.Type == Type.Number)
                return new Value(value._number - Number.One);

            var a = value._string;
            a--;
            return new Value(a);
        }


        public static Number Exponent(Value left, Value right)
        {
            if (left.Type == Type.Number && right.Type == Type.Number)
                return left._number.Exponent(right._number);
            else
                throw new ExecutionException("Attempted to exponent a string");
        }

        public static Number Exponent(Value _, YString __)
        {
            throw new ExecutionException("Attempted to exponent a string");
        }

        public static Number Exponent(Value left, Number right)
        {
            if (left.Type == Type.Number)
                return left._number.Exponent(right);
            else
                throw new ExecutionException("Attempted to exponent a string");
        }

        public static Number Exponent(Value left, bool right)
        {
            if (left.Type == Type.Number)
                return left._number.Exponent(right ? Number.One : Number.Zero);
            else
                throw new ExecutionException("Attempted to exponent a string");
        }


        public static bool operator !(Value value)
        {
            return value == Number.Zero;
        }

        public static Number operator -(Value value)
        {
            if (value.Type == Type.String)
                throw new ExecutionException("Attempted to negate a String value");

            return -value._number;
        }

        public static Number Abs(Value value)
        {
            if (value.Type == Type.String)
                throw new ExecutionException("Attempted to Abs a string value");

            return value._number.Abs();
        }

        public static Number Sqrt(Value value)
        {
            if (value.Type == Type.String)
                throw new ExecutionException("Attempted to Sqrt a string value");

            return value._number.Sqrt();
        }

        public static Number Sin(Value value)
        {
            if (value.Type == Type.String)
                throw new ExecutionException($"Attempted to `Sin` a string value");

            return value._number.Sin();
        }

        public static Number Cos(Value value)
        {
            if (value.Type == Type.String)
                throw new ExecutionException($"Attempted to `Cos` a string value");

            return value._number.Cos();
        }

        public static Number Tan(Value value)
        {
            if (value.Type == Type.String)
                throw new ExecutionException($"Attempted to `Tan` a string value");
            return value._number.Tan();
        }

        public static Number ArcTan(Value value)
        {
            if (value.Type == Type.String)
                throw new ExecutionException($"Attempted to `ATan` a string value");

            return value._number.ArcTan();
        }

        public static Number ArcSin(Value value)
        {
            if (value.Type == Type.String)
                throw new ExecutionException($"Attempted to `ASin` a string value");

            return value._number.ArcSin();
        }

        public static Number ArcCos(Value value)
        {
            if (value.Type == Type.String)
                throw new ExecutionException($"Attempted to `ACos` a string value");

            return value._number.ArcCos();
        }

        public BaseExpression ToConstant()
        {
            if (Type == Type.Number)
                return new ConstantNumber(_number);
            else
                return new ConstantString(String);
        }
    }
}

using System;
using System.Globalization;
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

        private readonly ReadOnlyMemory<char> _string;
        public string String
        {
            get
            {
                if (Type != Type.String)
                    throw new InvalidCastException($"Attempted to access value of type {Type} as a String");
                return _string.ToString();
            }
        }

        public Value(ReadOnlyMemory<char> str)
        {
            _string = str;
            _number = Number.Zero;
            Type = Type.String;
        }

        public Value(string str)
        {
            _string = str.AsMemory();
            _number = Number.Zero;
            Type = Type.String;
        }

        public Value(Number num)
        {
            _string = new Memory<char>(Array.Empty<char>());
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
                return Number.ToString(CultureInfo.InvariantCulture);
            else
                return String;
        }

        private ReadOnlyMemory<char> ToStringSpan()
        {
            if (Type == Type.Number)
                return Number.ToString(CultureInfo.InvariantCulture).AsMemory();
            else
                return _string;
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
                return (decimal)Number;
            else
                return String;
        }

        public bool Equals(Value other)
        {
            return (Type, other.Type) switch {
                (Type.Number, Type.Number) => Number == other.Number,
                (Type.String, Type.String) => CompareStringSpans(in this, in other) == 0,
                _ => false
            };
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
                {
                    foreach (var c in _string.Span)
                    {
                        hashCode += c;
                        hashCode *= 17;
                    }
                }
                else
                    hashCode *= _number.GetHashCode();

                return hashCode;
            }
        }

        public static Value operator <(Value left, Value right)
        {
            if (left.Type == Type.Number && right.Type == Type.Number)
                return new Value(left.Number < right.Number);

            return new Value(CompareStringSpans(in left, in right) < 0);
        }

        public static Value operator <=(Value left, Value right)
        {
            if (left.Type == Type.Number && right.Type == Type.Number)
                return new Value(left.Number <= right.Number);

            return new Value(CompareStringSpans(in left, in right) <= 0);
        }

        public static Value operator >(Value left, Value right)
        {
            if (left.Type == Type.Number && right.Type == Type.Number)
                return new Value(left.Number > right.Number);

            return new Value(CompareStringSpans(in left, in right) > 0);
        }

        public static Value operator >=(Value left, Value right)
        {
            if (left.Type == Type.Number && right.Type == Type.Number)
                return new Value(left.Number >= right.Number);

            return new Value(CompareStringSpans(in left, in right) >= 0);
        }

        private static int CompareStringSpans(in Value left, in Value right)
        {
            return left.ToStringSpan().Span.CompareTo(right.ToStringSpan().Span, StringComparison.Ordinal);
        }

        public static Value operator ==(Value left, Value right)
        {
            return left.Equals(right) ? Number.One : Number.Zero;
        }

        public static Value operator !=(Value left, Value right)
        {
            return !left.Equals(right) ? Number.One : Number.Zero;
        }

        public static Value operator -(Value left, Value right)
        {
            if (left.Type == Type.Number && right.Type == Type.Number)
                return left.Number - right.Number;

            var l = left.ToStringSpan();
            var r = right.ToStringSpan();
            var index = l.Span.LastIndexOf(r.Span);

            // Handle special cases by taking slices of the string if possible
            if (index == -1)
                return new Value(l);
            else if (index == 0)
                return new Value(l[r.Length..]);
            else if (index + r.Length == l.Length)
                return new Value(l[..^r.Length]);
            else
                return new Value(l.ToString().Remove(index, r.Length).AsMemory());
        }

        public static Value operator +(Value left, Value right)
        {
            if (left.Type == Type.Number && right.Type == Type.Number)
                return left.Number + right.Number;

            var l = left.ToStringSpan();
            var r = right.ToStringSpan();
            var result = new Memory<char>(new char[l.Length + r.Length]);
            l.CopyTo(result[..l.Length]);
            r.CopyTo(result[l.Length..]);
            return new Value(result);
        }

        public static Value operator *(Value left, Value right)
        {
            return (left.Type, right.Type) switch {
                (Type.Number, Type.Number) => new Value(left.Number * right.Number),
                (Type.String, Type.String) => throw new ExecutionException("Attempted to multiply strings"),
                _ => throw new ExecutionException("Attempted to multiply mixed types")
            };
        }

        public static Value operator /(Value left, Value right)
        {
            return (left.Type, right.Type) switch {
                (Type.Number, Type.Number) => new Value(left.Number / right.Number),
                (Type.String, Type.String) => throw new ExecutionException("Attempted to divide strings"),
                _ => throw new ExecutionException("Attempted to divide mixed types")
            };
        }

        public static Value operator &(Value left, Value right)
        {
            return new Value(left.ToBool() && right.ToBool());
        }

        public static Value operator |(Value left, Value right)
        {
            return new Value(left.ToBool() || right.ToBool());
        }

        public static Value operator %(Value left, Value right)
        {
            switch (left.Type, right.Type)
            {
                case (Type.Number, Type.Number):
                    if (right.Number == 0)
                        throw new ExecutionException("Divide by zero");
                    return new Value(left.Number % right.Number);

                case (Type.String, Type.String):
                    throw new ExecutionException("Attempted to modulo strings");

                default:
                    throw new ExecutionException("Attempted to modulo mixed types");
            }
        }

        public static Value operator ++(Value value)
        {
            if (value.Type == Type.Number)
                return new Value(value.Number + 1);

            var mem = new char[value._string.Length + 1];
            value._string.CopyTo(mem);
            mem[^1] = ' ';
            return new Value(mem.AsMemory());
        }

        public static Value operator --(Value value)
        {
            if (value.Type == Type.Number)
                return new Value(value.Number - Number.One);

            if (value._string.Length == 0)
                throw new ExecutionException("Attempted to decrement empty string");

            return new Value(value._string[..^1]);
        }

        public static Value Exponent(Value left, Value right)
        {
            switch (left.Type, right.Type)
            {
                case (Type.Number, Type.Number):
                    var v = Math.Pow((float)left.Number, (float)right.Number);

                    if (double.IsPositiveInfinity(v))
                        return new Value(Number.MaxValue);

                    if (double.IsNegativeInfinity(v) || double.IsNaN(v))
                        return new Value(Number.MinValue);

                    return new Value((decimal)v);

                case (Type.String, Type.String):
                    throw new ExecutionException("Attempted to exponent strings");

                default:
                    throw new ExecutionException("Attempted to exponent mixed types");
            }
        }

        public static Value operator !(Value value)
        {
            return value == Number.Zero;
        }

        public static Value operator -(Value value)
        {
            if (value.Type == Type.String)
                throw new ExecutionException("Attempted to negate a String value");

            return new Value(-value.Number);
        }

        public static Value Abs(Value value)
        {
            if (value.Type == Type.String)
                throw new ExecutionException("Attempted to Abs a string value");

            if (value.Number < 0)
                return -value;
            else
                return value;
        }

        public static Value Sqrt(Value value)
        {
            if (value.Type == Type.String)
                throw new ExecutionException("Attempted to Sqrt a string value");

            if (value.Number < 0)
                throw new ExecutionException("Attempted to Sqrt a negative value");

            return (Number)(decimal)Math.Sqrt((float)value.Number);
        }

        private static float ToDegrees(float radians)
        {
            const float rad2Deg = 360f / ((float)Math.PI * 2);
            return radians * rad2Deg;
        }

        private static float ToRadians(float degrees)
        {
            const float deg2Rad = (float)Math.PI * 2 / 360f;
            return degrees * deg2Rad;
        }

        public static Value Sin(Value value)
        {
            if (value.Type == Type.String)
                throw new ExecutionException($"Attempted to `Sin` a string value");

            var r = ToRadians((float)value.Number);
            var s = Math.Round(Math.Sin(r), 3);

            return new Value((Number)s);
        }

        public static Value Cos(Value value)
        {
            if (value.Type == Type.String)
                throw new ExecutionException($"Attempted to `Cos` a string value");

            var r = ToRadians((float)value.Number);
            var s = Math.Round(Math.Cos(r), 3);

            return new Value((Number)s);
        }

        public static Value Tan(Value value)
        {
            if (value.Type == Type.String)
                throw new ExecutionException($"Attempted to `Tan` a string value");
            if (value.Number == 90)
                return Number.MaxValue;

            var r = ToRadians((float)value.Number);
            var s = Math.Round(Math.Tan(r), 3);

            return new Value((Number)s);
        }

        public static Value ArcTan(Value value)
        {
            if (value.Type == Type.String)
                throw new ExecutionException($"Attempted to `ATan` a string value");

            return new Value((Number)ToDegrees((float)Math.Atan((float)value.Number)));
        }

        public static Value ArcSin(Value value)
        {
            if (value.Type == Type.String)
                throw new ExecutionException($"Attempted to `ASin` a string value");

            return new Value((Number)ToDegrees((float)Math.Asin((float)value.Number)));
        }

        public static Value ArcCos(Value value)
        {
            if (value.Type == Type.String)
                throw new ExecutionException($"Attempted to `ACos` a string value");

            return new Value((Number)ToDegrees((float)Math.Acos((float)value.Number)));
        }

        public BaseExpression ToConstant()
        {
            if (Type == Type.Number)
                return new ConstantNumber(Number);
            else
                return new ConstantString(String);
        }
    }
}

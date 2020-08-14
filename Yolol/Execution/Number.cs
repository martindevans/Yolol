using System;
using System.Globalization;

namespace Yolol.Execution
{
    public readonly struct Number
        : IEquatable<Number>
    {
        public const int Scale = 1000;
        public const int Decimals = 3;

        public static readonly Number MinValue = new Number(long.MinValue);
        public static readonly Number MaxValue = new Number(long.MaxValue);
        public static readonly Number One = new Number(1000);
        public static readonly Number Zero = new Number(0);

        private readonly long _value;

        private Number(long num)
        {
            _value = num;
        }

        public override string ToString()
        {
            return ((decimal)_value / Scale).ToString(CultureInfo.InvariantCulture);
        }

        public bool Equals(Number other)
        {
            return _value == other._value;
        }

        public override bool Equals(object obj)
        {
            return obj is Number other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_value);
        }

        public static Number Parse(string s)
        {
            // First check if the number is out of the valid range
            var d = double.Parse(s);
            if (d >= MaxValue._value)
                return MaxValue;
            else if (d <= MinValue._value)
                return MinValue;

            return decimal.Parse(s);
        }

        public static implicit operator Number(bool b)
        {
            return b ? One : Zero;
        }

        public static implicit operator Number(int i)
        {
            return new Number(i * (long)Scale);
        }

        public static explicit operator Number(double i)
        {
            var n = i * Scale;
            if (n > MaxValue._value)
                return MaxValue;
            if (n < MinValue._value)
                return MinValue;

            return new Number((long)n);
        }

        public static implicit operator Number(decimal d)
        {
            var n = d * Scale;
            if (n > MaxValue._value)
                return MaxValue;
            if (n < MinValue._value)
                return MinValue;

            return new Number((long)(d * Scale));
        }

        public static explicit operator decimal(Number n)
        {
            return ((decimal)n._value) / Scale;
        }

        public static explicit operator int(Number n)
        {
            return (int)(n._value / Scale);
        }

        public static explicit operator float(Number n)
        {
            return ((float)n._value) / Scale;
        }


        public static Number operator %(Number l, Number r)
        {
            if (r._value == 0)
                throw new ExecutionException("Modulus by zero");

            return new Number(l._value % r._value);
        }

        public static StaticError operator %(Number l, YString r)
        {
            return new StaticError("Attempted to modulus by a string");
        }

        public static Value operator %(Number l, Value r)
        {
            if (r.Type == Type.String)
                return new StaticError("Attempted to modulus by a string");
            else
                return l % r.Number;
        }

        public static Number operator %(Number l, bool r)
        {
            return l % (Number)r;
        }


        public static Number operator *(Number l, Number r)
        {
            return new Number((l._value * r._value) / Scale);
        }

        public static StaticError operator *(Number l, YString r)
        {
            return new StaticError("Attempted to multiply by a string");
        }

        public static Value operator *(Number l, Value r)
        {
            if (r.Type == Type.String)
                return new StaticError("Attempted to multiply by a string");
            else
                return l * r.Number;
        }

        public static Number operator *(Number l, bool r)
        {
            return l * (Number)r;
        }


        public static Number operator /(Number l, Number r)
        {
            if (r._value == 0)
                throw new ExecutionException("Divide by zero");

            return new Number((l._value * Scale) / r._value);
        }

        public static StaticError operator /(Number l, YString r)
        {
            return new StaticError("Attempted to divide by a string");
        }

        public static Value operator /(Number l, Value r)
        {
            if (r.Type == Type.String)
                return new StaticError("Attempted to divide by a string");
            else
                return l / r.Number;
        }

        public static Number operator /(Number l, bool r)
        {
            return l / (Number)r;
        }


        public static Number operator +(Number l, Number r)
        {
            return new Number(l._value + r._value);
        }

        public static YString operator +(Number l, YString r)
        {
            return new YString(l.ToString()) + r;
        }

        public static Value operator +(Number l, Value r)
        {
            return (Value)l + r;
        }

        public static Number operator +(Number l, bool r)
        {
            return l + (Number)r;
        }


        public static Number operator -(Number l, Number r)
        {
            return new Number(l._value - r._value);
        }

        public static YString operator -(Number l, YString r)
        {
            return new YString(l.ToString()) - r;
        }

        public static Value operator -(Number l, Value r)
        {
            return (Value)l - r;
        }

        public static Number operator -(Number l, bool r)
        {
            return l - (Number)r;
        }


        public static Number operator -(Number n)
        {
            return new Number(-n._value);
        }


        public static bool operator >(Number l, Number r)
        {
            return l._value > r._value;
        }

        public static bool operator >(Number l, YString r)
        {
            return new YString(l.ToString()) > r;
        }

        public static bool operator >(Number l, Value r)
        {
            if (r.Type == Type.Number)
                return l > r.Number;
            else
                return new YString(l.ToString()) > r.String;
        }

        public static bool operator >(Number l, bool r)
        {
            return l > (Number)r;
        }


        public static bool operator <(Number l, Number r)
        {
            return l._value < r._value;
        }

        public static bool operator <(Number l, YString r)
        {
            return new YString(l.ToString()) < r;
        }

        public static bool operator <(Number l, Value r)
        {
            if (r.Type == Type.Number)
                return l < r.Number;
            else
                return new YString(l.ToString()) < r.String;
        }

        public static bool operator <(Number l, bool r)
        {
            return l < (Number)r;
        }


        public static bool operator >=(Number l, Number r)
        {
            return l._value >= r._value;
        }

        public static bool operator >=(Number l, YString r)
        {
            return new YString(l.ToString()) >= r;
        }

        public static bool operator >=(Number l, Value r)
        {
            if (r.Type == Type.Number)
                return l >= r.Number;
            else
                return new YString(l.ToString()) >= r.String;
        }

        public static bool operator >=(Number l, bool r)
        {
            return l >= (Number)r;
        }


        public static bool operator <=(Number l, Number r)
        {
            return l._value <= r._value;
        }

        public static bool operator <=(Number l, YString r)
        {
            return new YString(l.ToString()) <= r;
        }

        public static bool operator <=(Number l, Value r)
        {
            if (r.Type == Type.Number)
                return l <= r.Number;
            else
                return new YString(l.ToString()) <= r.String;
        }

        public static bool operator <=(Number l, bool r)
        {
            return l <= (Number)r;
        }


        public static bool operator ==(Number l, Number r)
        {
            return l._value == r._value;
        }

        public static bool operator ==(Number _, YString __)
        {
            return false;
        }

        public static bool operator ==(Number l, Value r)
        {
            if (r.Type == Type.Number)
                return l == r.Number;
            else
                return false;
        }

        public static bool operator ==(Number l, bool r)
        {
            return l == (Number)r;
        }


        public static bool operator !=(Number l, Number r)
        {
            return l._value != r._value;
        }

        public static bool operator !=(Number _, YString __)
        {
            return true;
        }

        public static bool operator !=(Number l, Value r)
        {
            if (r.Type == Type.Number)
                return l != r.Number;
            else
                return true;
        }

        public static bool operator !=(Number l, bool r)
        {
            return l != (Number)r;
        }


        public Number Abs()
        {
            return new Number(Math.Abs(_value));
        }

        public Number Sqrt()
        {
            if (this < 0)
                throw new ExecutionException("Attempted to Sqrt a negative value");

            return (Number)(double)Math.Sqrt((float)this);
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

        public Number Sin()
        {
            var r = ToRadians((float)this);
            var s = Math.Round(Math.Sin(r), 3);

            return (Number)s;
        }

        public Number Cos()
        {
            var r = ToRadians((float)this);
            var s = Math.Round(Math.Cos(r), 3);

            return (Number)s;
        }

        public Number Tan()
        {
            if (this == 90)
                return MaxValue;

            var r = ToRadians((float)this);
            var s = Math.Round(Math.Tan(r), 3);

            return (Number)s;
        }

        public Number ArcTan()
        {
            return (Number)ToDegrees((float)Math.Atan((float)this));
        }

        public Number ArcSin()
        {
            return (Number)ToDegrees((float)Math.Asin((float)this));
        }

        public Number ArcCos()
        {
            return (Number)ToDegrees((float)Math.Acos((float)this));
        }


        public Number Exponent(Value right)
        {
            if (right.Type == Type.Number)
                return Exponent(right.Number);
            else
                throw new ExecutionException("Attempted to exponent a string");
        }

        public StaticError Exponent(YString _)
        {
            return new StaticError("Attempted to exponent a string");
        }

        public Number Exponent(Number number)
        {
            var v = Math.Pow((float)this, (float)number);

            if (double.IsPositiveInfinity(v))
                return MaxValue;

            if (double.IsNegativeInfinity(v) || double.IsNaN(v))
                return MinValue;

            return (Number)v;
        }

        public Number Exponent(bool right)
        {
            return Exponent((Number)right);
        }
    }
}

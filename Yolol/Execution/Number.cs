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

        private long Value { get; }

        private Number(long num)
        {
            Value = num;
        }

        public override string ToString()
        {
            return ((decimal)Value / Scale).ToString(CultureInfo.InvariantCulture);
        }

        public bool Equals(Number other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is Number other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }

        public static Number Parse(string s)
        {
            // First check if the number is out of the valid range
            var d = double.Parse(s);
            if (d >= MaxValue.Value)
                return MaxValue;
            else if (d <= MinValue.Value)
                return MinValue;

            return decimal.Parse(s);
        }

        public static implicit operator Number(int i)
        {
            return new Number(i * (long)Scale);
        }

        public static explicit operator Number(double i)
        {
            var n = i * Scale;
            if (n > MaxValue.Value)
                return MaxValue;
            if (n < MinValue.Value)
                return MinValue;

            return new Number((long)n);
        }

        public static implicit operator Number(decimal d)
        {
            var n = d * Scale;
            if (n > MaxValue.Value)
                return MaxValue;
            if (n < MinValue.Value)
                return MinValue;

            return new Number((long)(d * Scale));
        }

        public static explicit operator decimal(Number n)
        {
            return ((decimal)n.Value) / Scale;
        }

        public static explicit operator int(Number n)
        {
            return (int)(n.Value / Scale);
        }

        public static explicit operator float(Number n)
        {
            return ((float)n.Value) / Scale;
        }

        public static Number operator %(Number l, Number r)
        {
            if (r == 0)
                throw new ExecutionException("Modulus by zero");

            return new Number(l.Value % r.Value);
        }

        public static Number operator *(Number l, Number r)
        {
            return new Number((l.Value * r.Value) / Scale);
        }

        public static Number operator /(Number l, Number r)
        {
            if (r == Zero)
                throw new ExecutionException("Divide by zero");

            return new Number((l.Value * Scale) / r.Value);
        }

        public static Number operator +(Number l, Number r)
        {
            return new Number(l.Value + r.Value);
        }

        public static Number operator -(Number l, Number r)
        {
            return new Number(l.Value - r.Value);
        }

        public static Number operator -(Number n)
        {
            return new Number(-n.Value);
        }

        public static bool operator >(Number l, Number r)
        {
            return l.Value > r.Value;
        }

        public static bool operator <(Number l, Number r)
        {
            return l.Value < r.Value;
        }

        public static bool operator >=(Number l, Number r)
        {
            return l.Value >= r.Value;
        }

        public static bool operator <=(Number l, Number r)
        {
            return l.Value <= r.Value;
        }

        public static bool operator ==(Number l, Number r)
        {
            return l.Value == r.Value;
        }

        public static bool operator !=(Number l, Number r)
        {
            return l.Value != r.Value;
        }

        public Number Abs()
        {
            return new Number(Math.Abs(Value));
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

        public Number Exponent(Number number)
        {
            var v = Math.Pow((float)this, (float)number);

            if (double.IsPositiveInfinity(v))
                return MaxValue;

            if (double.IsNegativeInfinity(v) || double.IsNaN(v))
                return MinValue;

            return (Number)v;
        }
    }
}

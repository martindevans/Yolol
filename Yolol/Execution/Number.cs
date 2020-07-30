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

        public string ToString(CultureInfo culture)
        {
            return ((decimal)Value / Scale).ToString(culture);
        }

        public override string ToString()
        {
            return ToString(CultureInfo.InvariantCulture);
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
    }
}

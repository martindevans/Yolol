using System;
using System.Globalization;
using JetBrains.Annotations;

namespace Yolol.Execution
{
    public struct Number
        : IEquatable<Number>
    {
        public const decimal MaxValue = 9223372036854775.807m;
        public const decimal MinValue = -9223372036854775.808m;
        private const int Scale = 1000;

        public decimal Value { get; }

        public Number(decimal num)
        {
            Value = Math.Truncate(num * Scale) / Scale;

            if (Value > MaxValue)
                Value = MaxValue;
            if (Value < MinValue)
                Value = MinValue;
        }

        [NotNull] public string ToString(CultureInfo culture)
        {
            return Value.ToString(culture);
        }

        public bool Equals(Number other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is Number other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return ToString(CultureInfo.InvariantCulture);
        }

        public static implicit operator Number(int i)
        {
            return new Number(i);
        }

        public static implicit operator Number(decimal d)
        {
            return new Number(d);
        }

        public static Number operator *(Number l, Number r)
        {
            return new Number(l.Value * r.Value);
        }

        public static Number operator /(Number l, Number r)
        {
            return new Number(l.Value / r.Value);
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

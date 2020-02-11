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
        public const int Scale = 1000;

        public static readonly Number Min = new Number(MinValue);
        public static readonly Number Max = new Number(MaxValue);
        public static readonly Number One = new Number(1);
        public static readonly Number Zero = new Number(0);

        public decimal Value { get; }

        private Number(decimal num)
        {
            Value = num;
        }

        private Number Truncate()
        {
            return new Number(Math.Truncate(Value * Scale) / Scale);
        }

        private Number RangeCheck()
        {
            if (Value > MaxValue)
                return Max;

            if (Value < MinValue)
                return Min;

            return this;
        }

        private static Number SafeNew(decimal num)
        {
            return new Number(num).Truncate().RangeCheck();
        }

        [NotNull] public string ToString(CultureInfo culture)
        {
            return Value.ToString(culture);
        }

        [NotNull]  public override string ToString()
        {
            return ToString(CultureInfo.InvariantCulture);
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

        public static implicit operator Number(int i)
        {
            return new Number(i);
        }

        public static implicit operator Number(decimal d)
        {
            return SafeNew(d);
        }

        public static Number operator *(Number l, Number r)
        {
            return SafeNew(l.Value * r.Value);
        }

        public static Number operator /(Number l, Number r)
        {
            if (r == Zero)
                throw new ExecutionException("Divide by zero");

            return SafeNew(l.Value / r.Value);
        }

        public static Number operator +(Number l, Number r)
        {
            return SafeNew(l.Value + r.Value);
        }

        public static Number operator -(Number l, Number r)
        {
            return SafeNew(l.Value - r.Value);
        }

        public static Number operator -(Number n)
        {
            return new Number(-n.Value).RangeCheck();
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

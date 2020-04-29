using System;
using System.Globalization;

namespace Yolol.Execution
{
    public struct Number
        : IEquatable<Number>
    {
        public const int Scale = 1000;
        public const int Decimals = 3;

        public static readonly Number MinValue = new Number(-9223372036854775.808m);
        public static readonly Number MaxValue = new Number(9223372036854775.807m);
        public static readonly Number One = new Number(1);
        public static readonly Number Zero = new Number(0);

        public decimal Value { get; }

        private Number(decimal num)
        {
            Value = num;
        }

        private Number Truncate()
        {
            var d = Value;

            // https://stackoverflow.com/a/43639947/108234
            var r = Math.Round(d, Decimals);
            if (d > 0 && r > d)
                return new Number(r - new decimal(1, 0, 0, false, Decimals));
            else if (d < 0 && r < d)
                return new Number(r + new decimal(1, 0, 0, false, Decimals));

            return new Number(r);

            // Naieve approach
            //return new Number(Math.Truncate(Value * Scale) / Scale);
        }

        private Number RangeCheck()
        {
            if (Value > MaxValue.Value)
                return MaxValue;

            if (Value < MinValue.Value)
                return MinValue;

            return this;
        }

        private static Number SafeNew(decimal num)
        {
            return new Number(num).Truncate().RangeCheck();
        }

        public string ToString(CultureInfo culture)
        {
            return Value.ToString("0.###", culture);
        }

        public override string ToString()
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
            return HashCode.Combine(Value);
        }

        public static Number Parse(string s)
        {
            // First check if the number is so colossal that a decimal can't hold it
            var d = double.Parse(s);
            if (d >= (double)MaxValue.Value)
                return MaxValue;
            else if (d <= (double)MinValue.Value)
                return MinValue;

            // It's within the safe range, parse as decimal
            return SafeNew(decimal.Parse(s));
        }

        public static implicit operator Number(int i)
        {
            return new Number(i);
        }

        public static implicit operator Number(decimal d)
        {
            return SafeNew(d);
        }

        public static Number operator %(Number l, Number r)
        {
            var v = new Number(l.Value % r.Value);
            return v.Truncate();
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

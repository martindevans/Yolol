using System;
using System.Globalization;

namespace Yolol.Execution
{
    public struct Number
        : IEquatable<Number>
    {
        private decimal _value;
        public decimal Value
        {
            get => Math.Round(_value, 4, MidpointRounding.AwayFromZero);
            set => _value = Math.Truncate(value * 10000) / 10000;
        }

        public Number(decimal num)
        {
            _value = 0;
            Value = num;
        }

        public string ToString(CultureInfo culture)
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

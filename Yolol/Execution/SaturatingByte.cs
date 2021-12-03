using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Yolol.Execution
{
    [StructLayout(LayoutKind.Explicit)]
    internal readonly struct SaturatingByte
        : IEquatable<SaturatingByte>, IComparable<SaturatingByte>
    {
        [FieldOffset(0)]
        private readonly byte _value;

        public bool IsNonZero => _value > 0;
        public bool IsZero => _value == 0;

        public bool IsSaturated => _value == byte.MaxValue;

        public SaturatingByte(byte value)
        {
            _value = value;
        }

        public SaturatingByte(int value)
        {
            Debug.Assert(value >= 0);

            if (value > byte.MaxValue)
                _value = byte.MaxValue;
            else
                _value = (byte)value;
        }

        public override string ToString()
        {
            return IsSaturated
                ? "SATURATED"
                : _value.ToString();
        }

        public static SaturatingByte operator +(SaturatingByte a, SaturatingByte b)
        {
            var sum = a._value + b._value;

            if (sum > byte.MaxValue)
                return new SaturatingByte(byte.MaxValue);
            else
                return new SaturatingByte((byte)sum);
        }

        public static SaturatingByte operator -(SaturatingByte a, SaturatingByte b)
        {
            if (a.IsSaturated || b.IsSaturated)
                return new SaturatingByte(byte.MaxValue);

            var value = a._value - b._value;
            Debug.Assert(value >= 0);
            return new SaturatingByte((byte)value);
        }

        public static SaturatingByte operator -(SaturatingByte a, int b)
        {
            if (a.IsSaturated)
                return new SaturatingByte(byte.MaxValue);

            var value = a._value - b;
            Debug.Assert(value >= 0);
            return new SaturatingByte((byte)value);
        }

        public static SaturatingByte operator --(SaturatingByte a)
        {
            if (a.IsSaturated)
                return new SaturatingByte(byte.MaxValue);

            var value = a._value - 1;
            Debug.Assert(value >= 0);
            return new SaturatingByte((byte)value);
        }

        public static SaturatingByte operator ++(SaturatingByte a)
        {
            var value = a._value + 1;
            if (value >= byte.MaxValue)
                return new SaturatingByte(byte.MaxValue);
            else
                return new SaturatingByte((byte)value);
        }

        #region equality
        public bool Equals(SaturatingByte other)
        {
            return _value == other._value;
        }

        public override bool Equals(object? obj)
        {
            return obj is SaturatingByte other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public static bool operator ==(SaturatingByte left, SaturatingByte right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SaturatingByte left, SaturatingByte right)
        {
            return !left.Equals(right);
        }
        #endregion

        #region comparison
        public int CompareTo(SaturatingByte other)
        {
            return _value.CompareTo(other._value);
        }

        public int CompareTo(byte other)
        {
            return _value.CompareTo(other);
        }

        public static bool operator <(SaturatingByte a, SaturatingByte b)
        {
            return a._value < b._value;
        }

        public static bool operator >(SaturatingByte a, SaturatingByte b)
        {
            return a._value > b._value;
        }
        #endregion
    }
}

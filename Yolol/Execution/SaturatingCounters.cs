using System;
using System.Runtime.InteropServices;
using Yolol.Execution.Extensions;

namespace Yolol.Execution
{
    [StructLayout(LayoutKind.Explicit)]
    public struct SaturatingCounters
        : IEquatable<SaturatingCounters>
    {
        [FieldOffset(0)] internal SaturatingByte ZeroCount;
        public byte Zero => ZeroCount.Value;

        [FieldOffset(1)] internal SaturatingByte OnesCount;
        public byte One => OnesCount.Value;

        internal SaturatingCounters(SaturatingByte z, SaturatingByte o)
        {
            ZeroCount = z;
            OnesCount = o;
        }

        public SaturatingCounters(byte z, byte o)
        {
            ZeroCount = new SaturatingByte(z);
            OnesCount = new SaturatingByte(o);
        }

        public static SaturatingCounters FromString(string str)
        {
            return str.AsSpan().CountDigits();
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                ZeroCount,
                OnesCount
            );
        }

        public override bool Equals(object? obj)
        {
            if (obj is SaturatingCounters c)
                return Equals(c);

            return false;
        }

        public static bool operator ==(SaturatingCounters a, SaturatingCounters b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(SaturatingCounters a, SaturatingCounters b)
        {
            return !a.Equals(b);
        }

        public bool Equals(SaturatingCounters other)
        {
            return ZeroCount.Equals(other.ZeroCount) && OnesCount.Equals(other.OnesCount);
        }

        public static SaturatingCounters operator +(SaturatingCounters a, SaturatingCounters b)
        {
            return new SaturatingCounters(
                a.ZeroCount + b.ZeroCount,
                a.OnesCount + b.OnesCount
            );
        }

        public static SaturatingCounters operator -(SaturatingCounters a, SaturatingCounters b)
        {
            return new SaturatingCounters(
                a.ZeroCount - b.ZeroCount,
                a.OnesCount - b.OnesCount
            );
        }
    }
}

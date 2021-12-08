using System;
using System.Runtime.InteropServices;

namespace Yolol.Execution
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct SaturatingCounters
        : IEquatable<SaturatingCounters>
    {
        [FieldOffset(0)] public SaturatingByte ZeroCount;
        [FieldOffset(1)] public SaturatingByte OnesCount;

        public SaturatingCounters(SaturatingByte z, SaturatingByte o)
        {
            ZeroCount = z;
            OnesCount = o;
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

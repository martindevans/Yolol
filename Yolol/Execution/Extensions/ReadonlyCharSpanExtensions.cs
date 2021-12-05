using System;

namespace Yolol.Execution.Extensions
{
    internal static class ReadonlyCharSpanExtensions
    {
        public static SaturatingCounters CountDigits(this ReadOnlySpan<char> chars)
        {
            var z = 0;
            var o = 0;

            foreach (var c in chars)
            {
                if (c == '0')
                    z++;
                else if (c == '1')
                    o++;
            }

            return new SaturatingCounters(new SaturatingByte(z), new SaturatingByte(o));
        }

        public static SaturatingCounters CountDigits(this ReadOnlySpan<byte> chars)
        {
            var z = 0;
            var o = 0;

            foreach (var c in chars)
            {
                if (c == 48)        // 0
                    z++;
                else if (c == 49)   // 1
                    o++;
            }

            return new SaturatingCounters(new SaturatingByte(z), new SaturatingByte(o));
        }
    }
}

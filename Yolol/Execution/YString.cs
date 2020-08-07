using System;
using System.Runtime.CompilerServices;

namespace Yolol.Execution
{
    public readonly struct YString
        : IEquatable<YString>, IEquatable<string>
    {
        private readonly ReadOnlyMemory<char> _span;

        public int Length => _span.Length;

        public YString(ReadOnlyMemory<char> span)
        {
            _span = span;
        }

        public YString(string str)
        {
            _span = str.AsMemory();
        }

        public override string ToString()
        {
            return _span.ToString();
        }

        public override int GetHashCode()
        {
            var hashCode = 19;

            var span = _span.Span;
            for (var i = 0; i < span.Length; i++)
                hashCode = HashCode.Combine(hashCode, span[i]);

            return hashCode;
        }

        public bool Equals(YString other)
        {
            return this == other;
        }

        public bool Equals(string other)
        {
            if (other.Length != Length)
                return false;

            var span = _span.Span;
            var i = 0;
            foreach (var c in other)
                if (c != span[i++])
                    return false;

            return true;
        }

        public override bool Equals(object? obj)
        {
            return obj is YString other && Equals(other);
        }

        public static bool operator <(YString left, YString right)
        {
            return CompareStringSpans(left, right) < 0;
        }

        public static bool operator <=(YString left, YString right)
        {
            return CompareStringSpans(left, right) <= 0;
        }

        public static bool operator >(YString left, YString right)
        {
            return CompareStringSpans(left, right) > 0;
        }

        public static bool operator >=(YString left, YString right)
        {
            return CompareStringSpans(left, right) >= 0;
        }

        public static bool operator ==(YString left, YString right)
        {
            return CompareStringSpans(left, right) == 0;
        }

        public static bool operator !=(YString left, YString right)
        {
            return CompareStringSpans(left, right) != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CompareStringSpans(in YString left, in YString right)
        {
            return left._span.Span.CompareTo(right._span.Span, StringComparison.Ordinal);
        }

        public static YString operator +(YString left, YString right)
        {
            var result = new Memory<char>(new char[left.Length + right.Length]);
            left._span.CopyTo(result[..left.Length]);
            right._span.CopyTo(result[left.Length..]);
            return new YString(result);
        }

        public static YString operator +(YString left, char right)
        {
            var result = new Memory<char>(new char[left.Length + 1]);
            left._span.CopyTo(result[..left.Length]);
            result.Span[^1] = right;
            return new YString(result);
        }

        public static YString operator -(YString left, YString right)
        {
            var l = left._span;
            var r = right._span;
            var index = l.Span.LastIndexOf(r.Span);

            // Handle special cases by taking slices of the string if possible
            if (index == -1)
                return new YString(l);
            if (index == 0)
                return new YString(l[r.Length..]);
            if (index + r.Length == l.Length)
                return new YString(l[..^r.Length]);
            return new YString(l.ToString().Remove(index, r.Length).AsMemory());
        }

        public static YString operator --(YString value)
        {
            if (value.Length == 0)
                throw new ExecutionException("Attempted to decrement empty string");

            return new YString(value._span[..^1]);
        }

        public static YString operator ++(YString value)
        {
            return value + ' ';
        }
    }
}

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Yolol.Execution.Attributes;

namespace Yolol.Execution
{
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct YString
        : IEquatable<YString>, IEquatable<string>
    {
        [FieldOffset(0)]
        private readonly RopeSlice _span;

        public int Length => _span.Length;

        private YString(RopeSlice span)
        {
            _span = span;
        }

        public YString(string str)
        {
            _span = new RopeSlice(str);
        }

        public override string ToString()
        {
            return _span.ToString();
        }

        public override int GetHashCode()
        {
            return _span.GetHashCode();
        }

        public bool Equals(YString other)
        {
            return this == other;
        }

        public bool Equals(string? other)
        {
            return _span.Equals(other);
        }

        public override bool Equals(object? obj)
        {
            return obj is YString other && Equals(other);
        }


        public static YString Trim(YString str, int length)
        {
            return new YString(str._span.Trim(length));
        }


        public static bool operator <(YString left, YString right)
        {
            return CompareStringSpans(left, right) < 0;
        }

        public static bool operator <(YString left, Number right)
        {
            return CompareStringToNumber(left, right) < 0;
        }

        public static bool operator <(YString left, Value right)
        {
            if (right.Type == Type.Number)
                return left < right.Number;
            else
                return left < right.String;
        }

        public static bool operator <(YString left, bool right)
        {
            return left < (Number)right;
        }


        public static bool operator <=(YString left, YString right)
        {
            return CompareStringSpans(left, right) <= 0;
        }

        public static bool operator <=(YString left, Number right)
        {
            return CompareStringToNumber(left, right) <= 0;
        }

        public static bool operator <=(YString left, Value right)
        {
            if (right.Type == Type.Number)
                return left <= right.Number;
            else
                return left <= right.String;
        }

        public static bool operator <=(YString left, bool right)
        {
            return left <= (Number)right;
        }


        public static bool operator >(YString left, YString right)
        {
            return CompareStringSpans(left, right) > 0;
        }

        public static bool operator >(YString left, Number right)
        {
            return CompareStringToNumber(left, right) > 0;
        }

        public static bool operator >(YString left, Value right)
        {
            if (right.Type == Type.Number)
                return left > right.Number;
            else
                return left > right.String;
        }

        public static bool operator >(YString left, bool right)
        {
            return left > (Number)right;
        }


        public static bool operator >=(YString left, YString right)
        {
            return CompareStringSpans(left, right) >= 0;
        }

        public static bool operator >=(YString left, Number right)
        {
            return CompareStringToNumber(left, right) >= 0;
        }

        public static bool operator >=(YString left, Value right)
        {
            if (right.Type == Type.Number)
                return left >= right.Number;
            else
                return left >= right.String;
        }

        public static bool operator >=(YString left, bool right)
        {
            return left >= (Number)right;
        }


        public static bool operator ==(YString left, YString right)
        {
            return CompareStringSpans(left, right) == 0;
        }

        public static bool operator ==(YString _, Number __)
        {
            return false;
        }

        public static bool operator ==(YString left, Value right)
        {
            if (right.Type == Type.Number)
                return false;
            else
                return left == right.String;
        }

        public static bool operator ==(YString _, bool __)
        {
            return false;
        }


        public static bool operator !=(YString left, YString right)
        {
            return CompareStringSpans(left, right) != 0;
        }

        public static bool operator !=(YString _, Number __)
        {
            return true;
        }

        public static bool operator !=(YString left, Value right)
        {
            if (right.Type == Type.Number)
                return true;
            else
                return left != right.String;
        }

        public static bool operator !=(YString _, bool __)
        {
            return true;
        }


        private static int CompareStringToNumber(in YString left, in Number right)
        {
            unsafe
            {
                const int bufferSize = 128;
                var buffer = stackalloc char[bufferSize];
                var span = right.ToString(new Span<char>(buffer, bufferSize));

                return CompareStringSpans(left, span);
            }
        }

        private static int CompareStringSpans(in YString left, in YString right)
        {
            return left._span.CompareTo(right._span);
        }

        private static int CompareStringSpans(in YString left, in Span<char> right)
        {
            return left._span.CompareTo(right);
        }


        public static YString Add(YString l, YString r, int maxLength)
        {
            return new YString(RopeSlice.Concat(l._span, r._span, maxLength));
        }

        public static YString operator +(YString l, YString r)
        {
            return Add(l, r, int.MaxValue);
        }

        public static YString Add(Number l, YString r, int maxLength)
        {
            if (r.Length >= maxLength)
                return r;

            unsafe
            {
                const int bufferSize = 64;
                var buffer = stackalloc char[bufferSize];
                var leftSpan = l.ToString(new Span<char>(buffer, bufferSize));

                return new YString(RopeSlice.Concat(leftSpan, r._span, maxLength));
            }
        }

        public static YString Add(YString l, Number r, int maxLength)
        {
            if (l.Length >= maxLength)
                return l;

            unsafe
            {
                const int bufferSize = 64;
                var buffer = stackalloc char[bufferSize];
                var rightSpan = r.ToString(new Span<char>(buffer, bufferSize));

                return new YString(RopeSlice.Concat(l._span, rightSpan, maxLength));
            }
        }

        public static YString operator +(YString l, Number r)
        {
            return Add(l, r, int.MaxValue);
        }

        public static YString Add(Span<char> l, YString r, int maxLength)
        {
            return new YString(RopeSlice.Concat(l, r._span, maxLength));
        }

        public static YString operator +(Span<char> l, YString r)
        {
            return Add(l, r, int.MaxValue);
        }

        public static YString Add(YString l, Value r, int maxLength)
        {
            if (r.Type == Type.Number)
                return Add(l, r.Number, maxLength);
            else
                return Add(l, r.String, maxLength);
        }

        public static YString operator +(YString l, Value r)
        {
            return Add(l, r, int.MaxValue);
        }

        public static YString Add(YString l, char r, int maxLength)
        {
            return new YString(RopeSlice.Concat(l._span, r, maxLength));
        }

        public static YString operator +(YString l, char r)
        {
            return Add(l, r, int.MaxValue);
        }

        public static YString Add(YString l, bool r, int maxLength)
        {
            return new YString(RopeSlice.Concat(l._span, r ? '1' : '0', maxLength));
        }

        public static YString operator +(YString l, bool r)
        {
            return Add(l, r, int.MaxValue);
        }


        public static YString operator -(YString l, YString r)
        {
            return new YString(RopeSlice.Remove(l._span, r._span));
        }

        public static YString operator -(YString l, Number r)
        {
            unsafe
            {
                const int bufferSize = 128;
                var buffer = stackalloc char[bufferSize];
                var rightSpan = r.ToString(new Span<char>(buffer, bufferSize));

                return new YString(RopeSlice.Remove(l._span, rightSpan));
            }
        }

        public static YString operator -(YString l, Value r)
        {
            if (r.Type == Type.Number)
                return l - r.Number;
            else
                return l - r.String;
        }

        public static YString operator -(YString l, bool r)
        {
            return new YString(RopeSlice.Remove(l._span, r));
        }


        public static StaticError operator *(YString _, YString __)
        {
            throw new ExecutionException("Attempted to multiply a string");
        }

        public static StaticError operator *(YString _, Number __)
        {
            throw new ExecutionException("Attempted to multiply a string");
        }

        public static StaticError operator *(YString _, Value __)
        {
            throw new ExecutionException("Attempted to multiply a string");
        }

        public static StaticError operator *(YString _, bool __)
        {
            throw new ExecutionException("Attempted to multiply a string");
        }


        public static StaticError operator /(YString _, YString __)
        {
            throw new ExecutionException("Attempted to divide a string");
        }

        public static StaticError operator /(YString _, Number __)
        {
            throw new ExecutionException("Attempted to divide a string");
        }

        public static StaticError operator /(YString _, Value __)
        {
            throw new ExecutionException("Attempted to divide a string");
        }

        public static StaticError operator /(YString _, bool __)
        {
            throw new ExecutionException("Attempted to divide a string");
        }


        public static StaticError operator %(YString _, YString __)
        {
            throw new ExecutionException("Attempted to mod a string");
        }

        public static StaticError operator %(YString _, Number __)
        {
            throw new ExecutionException("Attempted to mod a string");
        }

        public static StaticError operator %(YString _, Value __)
        {
            throw new ExecutionException("Attempted to mod a string");
        }

        public static StaticError operator %(YString _, bool __)
        {
            throw new ExecutionException("Attempted to mod a string");
        }


        internal static bool WillDecThrow(YString str)
        {
            return str.Length == 0;
        }

        internal static YString UnsafeDecrement(YString value)
        {
            return new YString(value._span.Decrement());
        }

        public static YString operator --(YString value)
        {
            if (value.Length == 0)
                throw new ExecutionException("Attempted to decrement empty string");

            return new YString(value._span.Decrement());
        }


        public static YString Increment(YString value, int maxLength)
        {
            if (value.Length >= maxLength)
                return value;
            return value + ' ';
        }

        public static YString operator ++(YString value)
        {
            return Increment(value, int.MaxValue);
        }


#pragma warning disable CA1822 // Mark members as static
        public StaticError Exponent(Value _)
        {
            throw new ExecutionException("Attempted to exponent a string");
        }

        public StaticError Exponent(YString _)
        {
            throw new ExecutionException("Attempted to exponent a string");
        }

        public StaticError Exponent(Number _)
        {
            throw new ExecutionException("Attempted to exponent a string");
        }

        public StaticError Exponent(bool _)
        {
            throw new ExecutionException("Attempted to exponent a string");
        }


        public StaticError Sqrt()
        {
            throw new ExecutionException("Attempted to sqrt a string");
        }
#pragma warning restore CA1822 // Mark members as static

        public YString LastCharacter()
        {
            return LastCharacter(this);
        }

        [ErrorMetadata(nameof(WillDecThrow), unsafeAlternative: nameof(UnsafeLastCharacter))]
        internal static YString LastCharacter(YString str)
        {
            if (str.Length == 0)
                throw new ExecutionException("Attempted to decrement empty string");

            return UnsafeLastCharacter(str);
        }

        internal static YString UnsafeLastCharacter(YString str)
        {
            return new YString(str._span.PopLast());
        }
    }
}

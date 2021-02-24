using System;
using System.Runtime.CompilerServices;
using Yolol.Execution.Attributes;

namespace Yolol.Execution
{
    public readonly struct YString
        : IEquatable<YString>, IEquatable<string>
    {
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

        public bool Equals(string other)
        {
            return _span.Equals(other);
        }

        public override bool Equals(object? obj)
        {
            return obj is YString other && Equals(other);
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


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CompareStringSpans(in YString left, in YString right)
        {
            return left._span.CompareTo(right._span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CompareStringSpans(in YString left, in Span<char> right)
        {
            return left._span.CompareTo(right);
        }


        public static YString operator +(YString l, YString r)
        {
            return new YString(RopeSlice.Concat(l._span, r._span));
        }

        public static YString operator +(YString l, Number r)
        {
            unsafe
            {
                const int bufferSize = 128;
                var buffer = stackalloc char[bufferSize];
                var rightSpan = r.ToString(new Span<char>(buffer, bufferSize));

                return new YString(RopeSlice.Concat(l._span, rightSpan));
            }
        }

        public static YString operator +(Span<char> l, YString r)
        {
            return new YString(RopeSlice.Concat(l, r._span));
        }

        public static YString operator +(YString l, Value r)
        {
            if (r.Type == Type.Number)
                return l + r.Number;
            else
                return l + r.String;
        }

        public static YString operator +(YString l, char r)
        {
            return new YString(RopeSlice.Concat(l._span, r));
        }

        public static YString operator +(YString l, bool r)
        {
            return new YString(RopeSlice.Concat(l._span, r ? '1' : '0'));
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
            return new YString(RopeSlice.Remove(l._span, r ? '1' : '0'));
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

        [ErrorMetadata(nameof(WillDecThrow))]
        public static YString operator --(YString value)
        {
            if (value.Length == 0)
                throw new ExecutionException("Attempted to decrement empty string");

            return new YString(value._span.Decrement());
        }

        public static YString operator ++(YString value)
        {
            return value + ' ';
        }


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


        [ErrorMetadata(nameof(WillDecThrow))]
        public YString LastCharacter()
        {
            if (Length == 0)
                throw new ExecutionException("Attempted to decrement empty string");

            return new YString(_span.PopLast());
        }
    }
}

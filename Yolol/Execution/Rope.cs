using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Yolol.Execution
{
    internal class Rope
    {
        private readonly StringBuilder _builder;

        public int Length => _builder.Length;

        public Rope(int capacity = 64)
        {
            _builder = new StringBuilder(capacity);
        }

        public Rope(string initial, int capacity = 64)
        {
            _builder = new StringBuilder(initial, capacity);
        }

        public string ToString(int start, int length)
        {
            return _builder.ToString(start, length);
        }

        public void Append(Rope other, int sliceStart, int sliceLength)
        {
            _builder.Append(other._builder, sliceStart, sliceLength);
        }

        public void Append(ReadOnlySpan<char> other)
        {
            _builder.Append(other);
        }

        public char this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _builder[index];
        }

        public Rope CloneSlice(int start, int length, int capacity)
        {
            var r = new Rope(capacity);
            r._builder.Append(_builder, start, length);
            return r;
        }

        public Span<char> CopyTo(Span<char> destination, int sourceIndex, int sourceCount)
        {
            //ncrunch: no coverage start
            if (destination.Length < sourceCount)
                throw new ArgumentOutOfRangeException(nameof(destination), "Input span is too short for entire string");
            if (sourceCount - sourceIndex > Length)
                throw new ArgumentOutOfRangeException(nameof(sourceCount), "(sourceCount - sourceIndex) is longer than source");
            //ncrunch: no coverage end

            _builder.CopyTo(sourceIndex, destination, sourceCount);
            return destination[sourceCount..];
        }

        public bool SliceEquals(string other, int start, int count)
        {
            if (other.Length != count)
                return false;

            var i = 0;
            foreach (var c in other)
                if (c != _builder[start + i++])
                    return false;

            return true;
        }
    }

    internal readonly struct RopeSlice
        : IEquatable<string>
    {
        private readonly Rope? _rope;
        private readonly int _start;

        public int Length { get; }

        public char this[int index]
        {
            get
            {
                if (index < 0)
                    throw new IndexOutOfRangeException("Index is less than zero");
                if (index >= Length)
                    throw new IndexOutOfRangeException("Index is greater than or equal to length");

                // `_rope` can only be null if `Length == 0`. We've already checked against the length, so rope cannot be null.
                Debug.Assert(_rope != null);

                return _rope[_start + index];
            }
        }

        private RopeSlice(Rope rope, int start, int length)
        {
            _rope = rope;
            _start = start;
            Length = length;

#if DEBUG
            if (start < 0)
                throw new ArgumentOutOfRangeException(nameof(start), "start < 0");
            if (start >= rope.Length)
                throw new ArgumentOutOfRangeException(nameof(start), "start >= rope.Length");
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), "length < 0");
            if (start + length > rope.Length)
                throw new ArgumentOutOfRangeException(nameof(length), "start + length > rope.Length");
#endif
        }

        public RopeSlice(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                _rope = null;
                _start = 0;
                Length = 0;
            }
            else
            {
                _rope = new Rope(str);
                _start = 0;
                Length = str.Length;
            }
        }

        public override string ToString()
        {
            return _rope == null ? string.Empty : _rope.ToString(_start, Length);
        }

        public bool Equals(string other)
        {
            if (_rope == null)
                return string.Empty.Equals(other);

            return _rope.SliceEquals(other, _start, Length);
        }

        public override int GetHashCode()
        {
            var hashCode = 19;

            for (var i = 0; i < Length; i++)
                hashCode = HashCode.Combine(hashCode, this[i]);

            return hashCode;
        }

        public int CompareTo(in RopeSlice other)
        {
            if (Length == 0 || other.Length == 0)
                return Length - other.Length;

            // `_rope` can only be null if `Length == 0`. That was checked above for both ropes.
            Debug.Assert(_rope != null);
            Debug.Assert(other._rope != null);

            if (_rope == other._rope && _start == other._start && Length == other.Length)
                return 0;

            var l = Math.Min(Length, other.Length);
            for (var i = 0; i < l; i++)
            {
                var a = _rope[_start + i];
                var b = other._rope[other._start + i];

                if (a != b)
                    return a - b;
            }

            return Length - other.Length;
        }

        public int CompareTo(in Span<char> other)
        {
            if (Length == 0 || other.Length == 0)
                return Length - other.Length;

            // `_rope` can only be null if `Length == 0`. That was checked above.
            Debug.Assert(_rope != null);

            var l = Math.Min(Length, other.Length);
            for (var i = 0; i < l; i++)
            {
                var a = _rope[_start + i];
                var b = other[i];

                if (a != b)
                    return a - b;
            }

            return Length - other.Length;
        }

        public RopeSlice PopLast()
        {
            if (_rope == null)
                throw new InvalidOperationException("Attempted to `PopLast` on null `RopeSlice`");
            if (Length < 1)
                throw new InvalidOperationException("Attempted to `PopLast` on empty `RopeSlice`");

            return new RopeSlice(_rope, _start + Length - 1, 1);
        }

        public static RopeSlice Concat(in RopeSlice left, in RopeSlice right)
        {
            // If either part of the concat is an empty string (represented by a null rope) return the other half.
            if (left._rope == null)
                return right;
            if (right._rope == null)
                return left;

            // If the end of the left span points to the end of the underlying rope then
            // the rope can be extended in place.
            if (left._start + left.Length == left._rope.Length)
            {
                left._rope.Append(right._rope, right._start, right.Length);
                return new RopeSlice(left._rope, left._start, left.Length + right.Length);
            }

            // Create a copy of the rope to extend
            var rope = left._rope.CloneSlice(left._start, left.Length, left.Length + right.Length);
            rope.Append(right._rope, right._start, right.Length);
            return new RopeSlice(rope, 0, rope.Length);
        }

        public static RopeSlice Concat(in RopeSlice left, in ReadOnlySpan<char> right)
        {
            // If either part of the concat is an empty string return the other half.
            if (right.Length == 0)
                return left;
            if (left._rope == null)
            {
                var r = new Rope(right.Length);
                r.Append(right);
                return new RopeSlice(r, 0, r.Length);
            }

            // If the end of the left span points to the end of the underlying rope then
            // the rope can be extended in place.
            if (left._start + left.Length == left._rope.Length)
            {
                left._rope.Append(right);
                return new RopeSlice(left._rope, left._start, left.Length + right.Length);
            }

            // Create a copy of the rope to extend
            var rope = left._rope.CloneSlice(left._start, left.Length, left.Length + right.Length);
            rope.Append(right);
            return new RopeSlice(rope, 0, rope.Length);
        }

        public static RopeSlice Concat(in ReadOnlySpan<char> left, in RopeSlice right)
        {
            // If either part of the concat is an empty string return the other half.
            if (left.Length == 0)
                return right;
            if (right._rope == null)
            {
                var r = new Rope(left.Length);
                r.Append(left);
                return new RopeSlice(r, 0, r.Length);
            }

            // Create a new rope containing the data
            var rope = new Rope(left.Length + right.Length);
            rope.Append(left);
            rope.Append(right._rope, right._start, right.Length);
            return new RopeSlice(rope, 0, rope.Length);
        }

        public static RopeSlice Concat(in RopeSlice left, in char right)
        {
            unsafe
            {
                var r = right;
                return Concat(left, new Span<char>(&r, 1));
            }
        }

        public static RopeSlice Remove(in RopeSlice haystack, in RopeSlice needle)
        {
            // if the needle or haystack is an empty string early exit
            if (needle.Length == 0 || haystack.Length == 0)
                return haystack;

            // We just checked the length of both things is not zero. Which means the rope cannot be null either.
            Debug.Assert(haystack._rope != null);
            Debug.Assert(needle._rope != null);

            // If the left slice ends with the right slice then we can just return a shortened slice
            if (haystack.EndsWith(needle))
                return new RopeSlice(haystack._rope, haystack._start, haystack.Length - needle.Length);

            // Find the right slice within the left slice
            var index = haystack.LastIndexOf(needle);
            if (index < 0)
                return haystack;

            return Remove(in haystack, index, needle.Length);
        }

        public static RopeSlice Remove(in RopeSlice haystack, in ReadOnlySpan<char> needle)
        {
            if (needle.Length == 0 || haystack.Length == 0)
                return haystack;

            // We just checked the length of both things is not zero. Which means the rope cannot be null either.
            Debug.Assert(haystack._rope != null);

            // If the haystack slice ends with the needle then we can just return a shortened slice
            if (haystack.EndsWith(needle))
                return new RopeSlice(haystack._rope, haystack._start, haystack.Length - needle.Length);

            // Find the needle slice within the haystack
            var index = haystack.LastIndexOf(needle);
            if (index < 0)
                return haystack;

            return Remove(in haystack, index, needle.Length);
        }

        public static RopeSlice Remove(in RopeSlice haystack, char right)
        {
            unsafe
            {
                var r = right;
                return Remove(haystack, new Span<char>(&r, 1));
            }
        }

        private static RopeSlice Remove(in RopeSlice haystack, int startIndex, int needleLength)
        {
            Debug.Assert(haystack._rope != null);

            // If left slice _starts_ with the right string we can just offset the start of the slice
            if (startIndex == 0)
            {
                var start = haystack._start + needleLength;
                var length = haystack.Length - needleLength;
                if (length == 0)
                    start = 0;
                return new RopeSlice(haystack._rope, start, length);
            }

            // We'll have to make a new rope and remove it
            var rope = new Rope(haystack.Length);

            // Copy bit of haystack to the left of the needle into new rope
            rope.Append(haystack._rope, haystack._start, startIndex);

            // Copy bit of haystack to the right of the needle into new rope
            rope.Append(haystack._rope, haystack._start + startIndex + needleLength, haystack.Length - (startIndex + needleLength));

            return new RopeSlice(rope, 0, rope.Length);
        }

        private int LastIndexOf(in RopeSlice needle)
        {
            if (needle.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(needle), "Length of needle must be > 0");
            Debug.Assert(needle._rope != null);

            // if this is shorter than needle then this can't possibly contain needle
            if (Length < needle.Length)
                return -1;

            // todo: use better substring search
            // Copy the strings to two stack buffers and search in those buffers. This could be improved by running string search (e.g. aho-corasic directly
            // on the string builders, instead of incurring this extra copy. But C# doesn't have a convenient `LastIndexOf` method for StringBuilder and I
            // can't be bothered to build it right now!
            unsafe
            {
                var needleBuffer = stackalloc char[needle.Length];
                var needleSpan = new Span<char>(needleBuffer, needle.Length);
                needle._rope.CopyTo(needleSpan, needle._start, needle.Length);

                return LastIndexOf(needleSpan);
            }
        }

        private int LastIndexOf(in ReadOnlySpan<char> needle)
        {
            if (needle.Length <= 0)
                throw new ArgumentOutOfRangeException(nameof(needle), "Length of needle must be > 0");

            // if this is shorter than needle then this can't possibly contain needle
            if (Length < needle.Length)
                return -1;

            // We've asserted that the needle length is not zero, and that the length of this is not shorter than it. Hence the Length must be > 0 and the 
            // rope cannot be null.
            Debug.Assert(_rope != null);

            // todo: use better substring search
            // Copy the strings to two stack buffers and search in those buffers. This could be improved by running string search (e.g. aho-corasic directly
            // on the string builders, instead of incurring this extra copy. But C# doesn't have a convenient `LastIndexOf` method for StringBuilder and I
            // can't be bothered to build it right now!
            unsafe
            {
                var haystackBuffer = stackalloc char[Length];
                var haystackSpan = new Span<char>(haystackBuffer, Length);
                _rope.CopyTo(haystackSpan, _start, Length);

                return haystackSpan.LastIndexOf(needle);
            }
        }

        private bool EndsWith(in RopeSlice right)
        {
            try
            {
                if (Length < right.Length)
                    return false;

                for (var i = 0; i < right.Length; i++)
                {
                    var c = right[right.Length - 1 - i];
                    var d = this[Length - 1 - i];

                    if (c != d)
                        return false;
                }

                return true;
            }
            catch
            {
                Console.WriteLine(this);
                Console.WriteLine(right);
                throw;
            }
        }

        private bool EndsWith(in ReadOnlySpan<char> right)
        {
            if (Length < right.Length)
                return false;

            for (var i = 1; i <= right.Length; i++)
            {
                var c = right[^i];
                var d = this[^i];

                if (c != d)
                    return false;
            }

            return true;
        }

        public RopeSlice Decrement()
        {
            if (Length == 0)
                throw new InvalidOperationException("Cannot decrement empty slice");
            Debug.Assert(_rope != null);

            return new RopeSlice(_rope, _start, Length - 1);
        }
    }
}

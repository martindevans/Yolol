using System;
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

        public char this[int index]
        {
            get
            {
                if (index < 0)
                    throw new IndexOutOfRangeException("Index is less than zero");
                if (index >= Length)
                    throw new IndexOutOfRangeException("Index is greater than or equal to length");

                return _builder[index];
            }
        }

        public Rope CloneSlice(int start, int length)
        {
            var r = new Rope(length);
            r._builder.Append(_builder, start, length);
            return r;
        }

        public void RemoveRange(int index, int length)
        {
            _builder.Remove(index, length);
        }

        public void Append(char right)
        {
            _builder.Append(right);
        }

        public Span<char> CopyTo(Span<char> destination, int sourceIndex, int sourceCount)
        {
            if (destination.Length < sourceCount)
                throw new ArgumentOutOfRangeException(nameof(destination), "Input span is too short for entire string");
            if (sourceCount - sourceIndex > Length)
                throw new ArgumentOutOfRangeException(nameof(sourceCount), "(sourceCount - sourceIndex) is longer than source");

            _builder.CopyTo(sourceIndex, destination, sourceCount);
            return destination.Slice(sourceCount);
        }
    }

    internal readonly struct RopeSlice
        : IEquatable<string>
    {
        private readonly Rope _rope;
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

                return _rope[_start + index];
            }
        }

        private RopeSlice(Rope rope, int start, int length)
        {
            _rope = rope;
            _start = start;
            Length = length;
        }

        public RopeSlice(string str)
        {
            _rope = new Rope(str);
            _start = 0;
            Length = str.Length;
        }

        public override string ToString()
        {
            return _rope.ToString(_start, Length);
        }

        public bool Equals(string other)
        {
            if (other.Length != Length)
                return false;

            var i = 0;
            foreach (var c in other)
                if (c != _rope[_start + i++])
                    return false;

            return true;
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

            // We don't really need to copy into stack buffers here, instead we could directly compare the StringBuilders character by character.
            unsafe
            {
                var haystackBuffer = stackalloc char[Length];
                var haystackSpan = new Span<char>(haystackBuffer, Length);
                _rope.CopyTo(haystackSpan, _start, Length);

                var needleBuffer = stackalloc char[other.Length];
                var needleSpan = new Span<char>(needleBuffer, other.Length);
                other._rope.CopyTo(needleSpan, other._start, other.Length);

                return MemoryExtensions.CompareTo(haystackSpan, needleSpan, StringComparison.Ordinal);
            }
        }

        public static RopeSlice Concat(in RopeSlice left, in RopeSlice right)
        {
            // If the end of the left span points to the end of the underlying rope then
            // the rope can be extended in place.
            if (left._start + left.Length == left._rope.Length)
            {
                left._rope.Append(right._rope, right._start, right.Length);
                return new RopeSlice(left._rope, left._start, left.Length + right.Length);
            }

            // Create a copy of the rope to extend
            var rope = left._rope.CloneSlice(left._start, left.Length);
            rope.Append(right._rope, right._start, right.Length);
            return new RopeSlice(rope, 0, rope.Length);
        }

        public static RopeSlice Concat(in RopeSlice left, in char right)
        {
            // If the left slice reaches to the end of the rope then we can extend the rope in place
            if (left._start + left.Length == left._rope.Length)
            {
                left._rope.Append(right);
                return new RopeSlice(left._rope, left._start, left.Length + 1);
            }

            // Clone the slice and extend it
            var rope = left._rope.CloneSlice(left._start, left.Length);
            rope.Append(right);
            return new RopeSlice(rope, left._start, left.Length + 1);
        }

        public static RopeSlice Remove(in RopeSlice left, in RopeSlice right)
        {
            if (right.Length == 0)
                return left;

            // If the left slice ends with the right slice then we can just return a shortened slice
            if (left.EndsWith(right))
                return new RopeSlice(left._rope, left._start, left.Length - right.Length);

            // Find the right slice within the left slice
            var index = left.LastIndexOf(right);
            if (index < 0)
                return left;

            // If left slice _starts_ with the right string we can just offset the start of the slice
            if (index == 0)
                return new RopeSlice(left._rope, left._start + right.Length, left.Length - right.Length);

            // We'll have to make a new rope and remove it
            var rope = left._rope.CloneSlice(left._start, left.Length);
            rope.RemoveRange(index, right.Length);
            return new RopeSlice(rope, left._start, left.Length - right.Length);
        }

        public static RopeSlice Remove(in RopeSlice left, in ReadOnlySpan<char> right)
        {
            if (right.Length == 0)
                return left;

            // If the left slice ends with the right slice then we can just return a shortened slice
            if (left.EndsWith(right))
                return new RopeSlice(left._rope, left._start, left.Length - right.Length);

            // Find the right slice within the left slice
            var index = left.LastIndexOf(right);
            if (index < 0)
                return left;

            // If left slice _starts_ with the right string we can just offset the start of the slice
            if (index == 0)
                return new RopeSlice(left._rope, left._start + right.Length, left.Length - right.Length);

            // We'll have to make a new rope and remove it
            var rope = left._rope.CloneSlice(left._start, left.Length);
            rope.RemoveRange(index, right.Length);
            return new RopeSlice(rope, left._start, left.Length - right.Length);
        }

        public static RopeSlice Remove(in RopeSlice left, char right)
        {
            // If the left slice ends with the right slice then we can just return a shortened slice
            if (left.EndsWith(right))
                return new RopeSlice(left._rope, left._start, left.Length - 1);

            // Find the right slice within the left slice
            var index = left.LastIndexOf(right);
            if (index < 0)
                return left;

            // If left slice _starts_ with the right string we can just offset the start of the slice
            if (index == 0)
                return new RopeSlice(left._rope, left._start + 1, left.Length - 1);

            // We'll have to make a new rope and remove it
            var rope = left._rope.CloneSlice(left._start, left.Length);
            rope.RemoveRange(index, 1);
            return new RopeSlice(rope, left._start, left.Length - 1);
        }

        private int LastIndexOf(in RopeSlice needle)
        {
            if (needle.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(needle), "Length of needle must be > 0");

            // if this is shorter than needle then this can't possibly contain needle
            if (Length < needle.Length)
                return -1;

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
            if (needle.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(needle), "Length of needle must be > 0");

            // if this is shorter than needle then this can't possibly contain needle
            if (Length < needle.Length)
                return -1;

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

        private int LastIndexOf(in char needle)
        {
            // if this is shorter than needle then this can't possibly contain needle
            if (Length < 1)
                return -1;

            for (var i = Length - 1; i >= 0; i--)
                if (this[i] == needle)
                    return i;

            return -1;
        }

        private bool EndsWith(in RopeSlice right)
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

        private bool EndsWith(in char right)
        {
            if (Length < 1)
                return false;

            return this[^1] == right;
        }

        public RopeSlice Decrement()
        {
            if (Length == 0)
                throw new InvalidOperationException("Cannot decrement empty slice");

            return new RopeSlice(_rope, _start, Length - 1);
        }
    }
}

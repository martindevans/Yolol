using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Yolol.Execution.Extensions;

namespace Yolol.Execution
{
    /// <summary>
    /// A mutable string of characters
    /// </summary>
    internal class Rope
    {
        private enum Variant
        {
            /// <summary>
            /// There are `count` characters stored in `_chars`
            /// </summary>
            Flat,

            /// <summary>
            /// `_chars` is null and the string is the concatenation of `_left` and `_right`
            /// </summary>
            Concat,
        }

        #region data fields
        /// <summary>
        /// Depending on the value of this field certain other fields may be null. See comments on Variant struct.
        /// wtb discriminated union.
        /// </summary>
        private Variant _variant;

        private int _count;
        private Memory<char> _chars;

        private Slice _left;
        private Slice _right;
        #endregion

        public int Length => _variant == Variant.Flat ? _count : _left.Length + _right.Length;

        #region constructors
        public Rope(int capacity = 64)
        {
            _chars = new char[capacity];
            _variant = Variant.Flat;
        }

        public Rope(ReadOnlySpan<char> initial, int capacity = 64)
        {
            _chars = new char[Math.Max(initial.Length * 2, capacity)];
            _variant = Variant.Flat;

            initial.CopyTo(_chars.Span);
            _count = initial.Length;
        }

        public Rope(Slice left, Slice right)
        {
            _left = left;
            _right = right;
            _variant = Variant.Concat;

            _chars = null;
            _count = 0;

            if (IsDepthGreaterThan(5))
                Flatten();
        }
        #endregion

        /// <summary>
        /// Transform into the flat variant
        /// </summary>
        private void Flatten()
        {
            if (_variant == Variant.Flat)
                return;

            _chars = new char[_left.Length + _right.Length + 64];
            _variant = Variant.Flat;

            _left.CopyTo(_chars.Span);
            _right.CopyTo(_chars.Span[_left.Length..]);
            _count = _left.Length + _right.Length;

            _left = default;
            _right = default;
        }

        public void Append(ReadOnlySpan<char> other)
        {
            Flatten();

            if (_count + other.Length > _chars.Length)
            {
                var expanded = new char[Math.Max(_chars.Length * 2, _count + other.Length)];
                _chars.CopyTo(expanded);
                _chars = expanded;
            }

            other.CopyTo(_chars[_count..].Span);
            _count += other.Length;
        }

        public Rope CloneSlice(int start, int length, int capacity = 64)
        {
            // Create a new rope with enough capacity for the relevant data
            var r = new Rope(Math.Max(length, capacity));

            // Copy from this slice into the flat buffer
            Debug.Assert(r._chars.Length >= length);
            CopySlice(start, length, r._chars.Span);
            r._count = length;

            return r;
        }

        public ReadOnlySpan<char> GetSpan(int sourceIndex, int sourceCount)
        {
            Flatten();
            Debug.Assert(sourceCount - sourceIndex <= _count);
            return _chars.Slice(sourceIndex, sourceCount).Span;
        }

        internal void CopySlice(int start, int length, Span<char> destination)
        {
            if (_variant == Variant.Flat)
            {
                _chars.Slice(start, length).Span.CopyTo(destination);
            }
            else if (_variant == Variant.Concat)
            {
                var end = start + length;

                // |---------------|------------|
                //      |-----|
                //          |----------|
                //                    |---|

                // Copy from the left span
                var leftCopy = Math.Max(0, Math.Min(_left.Length - start, length));
                _left.CopyTo(start, leftCopy, destination);

                // Copy from the right span
                if (end > _left.Length)
                {
                    var rStart = Math.Max(0, start - _left.Length);
                    var rEnd = end - _left.Length;
                    var rLen = rEnd - rStart;
                    if (rLen > 0)
                        _right.CopyTo(rStart, rLen, destination[leftCopy..]);
                }
            }
            else
                throw new InvalidOperationException($"Unknown Rope variant: `{_variant}`");
        }

        public bool IsDepthGreaterThan(int limit)
        {
            if (_variant == Variant.Flat)
                return false;
            if (limit == 0)
                return true;
            return _left.IsDepthGreaterThan(limit - 1) || _right.IsDepthGreaterThan(limit - 1);
        }
    }
    
    internal readonly struct Slice
    {
        private readonly Rope _rope;
        private readonly int _start;
        public readonly int Length;

        public Slice(Rope rope, int start, int length)
        {
            _rope = rope;
            _start = start;
            Length = length;
        }

        public bool IsDepthGreaterThan(int limit)
        {
            return _rope.IsDepthGreaterThan(limit);
        }

        internal void CopyTo(Span<char> destination)
        {
            _rope.CopySlice(_start, Length, destination);
        }

        internal void CopyTo(int start, int length, Span<char> destination)
        {
            if (length == 0)
                return;

            if (start < 0)
                throw new ArgumentOutOfRangeException(nameof(start), "Must be greater than zero");
            if (start >= Length)
                throw new ArgumentOutOfRangeException(nameof(start), "Must be less than length");
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), "Must be greater than zero");
            if (length > Length - start)
                throw new ArgumentOutOfRangeException(nameof(length), "Must be within slice");

            _rope.CopySlice(_start + start, length, destination);
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    internal readonly struct RopeSlice
        : IEquatable<string>
    {
        [FieldOffset(0)]
        private readonly ushort _start;

        //There are two spare bytes here, available for extra metadata
        //[FieldOffset(2)] private readonly byte _spare0;
        //[FieldOffset(3)] private readonly byte _spare1;

        [field: FieldOffset(4)]
        public int Length { get; }

        [FieldOffset(8)]
        private readonly Rope? _rope;

        public ReadOnlySpan<char> AsSpan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return _rope == null ? ReadOnlySpan<char>.Empty : AsSpanUnchecked;
            }
        }
        private ReadOnlySpan<char> AsSpanUnchecked
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                Debug.Assert(_rope != null);
                return _rope.GetSpan(_start, Length);
            }
        }

        #region constructors
        private RopeSlice(Rope rope, int start, int length)
        {
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

            _rope = rope;

            Length = length;

            // If the start offset is too large create a new rope with the relevant data at offset 0
            if (start > ushort.MaxValue)
            {
                _rope = rope.CloneSlice(start, length);
                _start = 0;
            }
            else
            {
                _start = (ushort)start;
            }
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
        #endregion

        public override string ToString()
        {
            return AsSpan.ToString();
        }

        public bool Equals(string? other)
        {
            return AsSpan.SequenceEqual(other ?? ReadOnlySpan<char>.Empty);
        }

        public override int GetHashCode()
        {
            HashCode c = new HashCode();

            var s = AsSpan;
            for (int i = 0; i < s.Length; i++)
                c.Add(s[i]);

            return c.ToHashCode();
        }

        #region CompareTo
        public int CompareTo(in RopeSlice other)
        {
            if (Length == 0 || other.Length == 0)
                return Length - other.Length;

            // `_rope` can only be null if `Length == 0`. That was checked above for both ropes.
            Debug.Assert(_rope != null);
            Debug.Assert(other._rope != null);

            // Check if both items are identical slices from the same rope
            if (_rope == other._rope && _start == other._start && Length == other.Length)
                return 0;

            return CompareTo(other.AsSpanUnchecked);
        }

        public int CompareTo(in ReadOnlySpan<char> other)
        {
            if (Length == 0 || other.Length == 0)
                return Length - other.Length;

            // `_rope` can only be null if `Length == 0`. That was checked above.
            Debug.Assert(_rope != null);

            return AsSpanUnchecked.CompareTo(other, StringComparison.Ordinal);
        }
        #endregion

        private char LastCharUnchecked()
        {
            Debug.Assert(_rope != null);
            Span<char> lastChar = stackalloc char[1];
            _rope.CopySlice(_start + Length - 1, 1, lastChar);
            return lastChar[0];
        }

        public RopeSlice PopLast()
        {
            if (Length < 1)
                throw new InvalidOperationException("Attempted to `PopLast` on empty `RopeSlice`");
            Debug.Assert(_rope != null);

            return new RopeSlice(_rope, _start + Length - 1, 1);
        }

        #region concat
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RopeSlice Concat(in RopeSlice left, in RopeSlice right, int maxLength)
        {
            // If either part of the concat is an empty string (represented by a null rope) return the other half.
            if (left._rope == null || left.Length == 0)
                return right;
            if (right._rope == null || right.Length == 0 || left.Length >= maxLength)
                return left;

            // Create slices for the two parts, respecting the length limit
            var sliceLeft = new Slice(left._rope, left._start, left.Length);
            var sliceRight = new Slice(right._rope, right._start, Math.Min(right.Length, maxLength - left.Length));

            // Create a new rope concatenating the two slices
            var rope = new Rope(sliceLeft, sliceRight);
            return new RopeSlice(
                rope,
                0,
                rope.Length
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RopeSlice Concat(in RopeSlice left, ReadOnlySpan<char> right, int maxLength)
        {
            // If part of the concat is an empty string return the other half.
            if (right.Length == 0)
                return left;
            
            // Trim right side down to length
            right = right[..Math.Min(maxLength - left.Length, right.Length)];

            // If left is null just return right
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RopeSlice Concat(ReadOnlySpan<char> left, in RopeSlice right, int maxLength)
        {
            // If part of the concat is an empty string return the other half.
            if (left.Length == 0)
                return right;

            // Trim left down to length and count the digits
            left = left[..Math.Min(left.Length, maxLength)];
            var (lz, lo) = left.CountDigits();

            // If the right still is null (i.e. zero length) return a new rope which is just the left string
            if (right._rope == null || right.Length == 0)
                return new RopeSlice(new Rope(left), 0, left.Length);

            // Create a new rope and append the left part
            var rope = new Rope(Math.Min(maxLength, left.Length + right.Length));
            rope.Append(left);

            // Now append as much of the right as possible
            if (rope.Length < maxLength)
            {
                var rSpan = right.AsSpanUnchecked[..Math.Min(right.Length, maxLength - left.Length)];
                rope.Append(rSpan);
            }
            return new RopeSlice(rope, 0, rope.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RopeSlice Concat(in RopeSlice left, in char right, int maxLength)
        {
            if (left.Length >= maxLength)
                return left;

            unsafe
            {
                var r = right;
                return Concat(left, new Span<char>(&r, 1), maxLength);
            }
        }
        #endregion

        #region remove
        public static RopeSlice Remove(in RopeSlice haystack, in RopeSlice needle)
        {
            return Remove(haystack, needle.AsSpan);
        }

        public static RopeSlice Remove(in RopeSlice haystack, in ReadOnlySpan<char> needle)
        {
            // Any empty needle is a no-op
            if (needle.Length == 0 || haystack.Length == 0)
                return haystack;

            // We just checked the length of both things is not zero. Which means the rope cannot be null either.
            Debug.Assert(haystack._rope != null);

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

            // If the left slice _ends_ with the right string we can just shorten the slice
            if (startIndex + needleLength == haystack.Length)
                return new RopeSlice(haystack._rope, haystack._start, haystack.Length - needleLength);

            // Make a new rope out of the two parts
            var leftPart = new Slice(haystack._rope, haystack._start, startIndex);
            var rightPart = new Slice(haystack._rope, haystack._start + startIndex + needleLength, haystack.Length - needleLength - startIndex);
            var concat = new Rope(leftPart, rightPart);
            return new RopeSlice(concat, 0, concat.Length);
        }
        #endregion

        #region search
        private int LastIndexOf(in RopeSlice needle)
        {
            Debug.Assert(needle.Length > 0);
            Debug.Assert(needle._rope != null);

            // if this is shorter than needle then this can't possibly contain needle
            if (Length < needle.Length)
                return -1;

            return LastIndexOf(needle.AsSpanUnchecked);
        }

        private int LastIndexOf(in ReadOnlySpan<char> needle)
        {
            Debug.Assert(needle.Length > 0);

            // if haystack is shorter than needle then haystack can't possibly contain needle
            if (Length < needle.Length)
                return -1;

            // We've asserted that the needle length is not zero, and that the length of this is not shorter than it. Hence the Length must be > 0 and the 
            // rope cannot be null.
            Debug.Assert(_rope != null);

            return AsSpanUnchecked.LastIndexOf(needle);
        }
        #endregion

        public RopeSlice Decrement()
        {
            if (Length < 1)
                throw new InvalidOperationException("Cannot decrement empty slice");
            Debug.Assert(_rope != null);

            return new RopeSlice(_rope, _start, Length - 1);
        }

        public RopeSlice Trim(int length)
        {
            if (Length <= length || _rope == null)
                return this;

            return new RopeSlice(_rope, _start, length);
        }
    }
}

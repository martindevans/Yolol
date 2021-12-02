using System;
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

        [FieldOffset(2)] private readonly SaturatingByte _zeroCount;
        [FieldOffset(3)] private readonly SaturatingByte _onesCount;

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
        private RopeSlice(Rope rope, int start, int length, SaturatingByte zeroCount, SaturatingByte onesCount)
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
            _zeroCount = zeroCount;
            _onesCount = onesCount;

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

                _zeroCount = default;
                _onesCount = default;
            }
            else
            {
                _rope = new Rope(str);
                _start = 0;
                Length = str.Length;

                (_zeroCount, _onesCount) = str.AsSpan().CountDigits();
            }
        }

        public RopeSlice(string str, int zeroes, int ones)
        {
            if (string.IsNullOrEmpty(str))
            {
                _rope = null;
                _start = 0;
                Length = 0;

                _zeroCount = default;
                _onesCount = default;
            }
            else
            {
                _rope = new Rope(str);
                _start = 0;
                Length = str.Length;

                _zeroCount = new SaturatingByte(zeroes);
                _onesCount = new SaturatingByte(ones);
            }
        }

        internal RopeSlice(string str, SaturatingByte zeroCount, SaturatingByte oneCount)
        {
            if (string.IsNullOrEmpty(str))
            {
                _rope = null;
                _start = 0;
                Length = 0;

                _zeroCount = default;
                _onesCount = default;
            }
            else
            {
                _rope = new Rope(str);
                _start = 0;
                Length = str.Length;

                _zeroCount = zeroCount;
                _onesCount = oneCount;
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

            var zc = default(SaturatingByte);
            var oc = default(SaturatingByte);
            var lastChar = LastCharUnchecked();
            if (lastChar == '0')
                zc = new SaturatingByte(1);
            else if (lastChar == '1')
                oc = new SaturatingByte(1);

            return new RopeSlice(_rope, _start + Length - 1, 1, zc, oc);
        }

        #region concat
        public static RopeSlice Concat(in RopeSlice left, in RopeSlice right)
        {
            // If either part of the concat is an empty string (represented by a null rope) return the other half.
            if (left._rope == null || left.Length == 0)
                return right;
            if (right._rope == null || right.Length == 0)
                return left;

            //// If the end of the left span points to the end of the underlying rope then
            //// the rope can be extended in place.
            //if (left._start + left.Length == left._rope.Length)
            //{
            //    left._rope.Append(right.AsSpanUnchecked);
            //    return new RopeSlice(
            //        left._rope,
            //        left._start, left.Length + right.Length,
            //        left._zeroCount + right._zeroCount,
            //        left._onesCount + right._onesCount
            //    );
            //}

            var rope = new Rope(
                new Slice(left._rope, left._start, left.Length),
                new Slice(right._rope, right._start, right.Length)
            );
            return new RopeSlice(
                rope,
                0,
                left.Length + right.Length,
                left._zeroCount + right._zeroCount,
                left._onesCount + right._onesCount
            );
        }

        public static RopeSlice Concat(in RopeSlice left, in ReadOnlySpan<char> right)
        {
            // If part of the concat is an empty string return the other half.
            if (right.Length == 0)
                return left;
            
            var (rz, ro) = right.CountDigits();
            if (left._rope == null)
            {
                var r = new Rope(right.Length);
                r.Append(right);
                return new RopeSlice(r, 0, r.Length, rz, ro);
            }

            // If the end of the left span points to the end of the underlying rope then
            // the rope can be extended in place.
            if (left._start + left.Length == left._rope.Length)
            {
                left._rope.Append(right);
                return new RopeSlice(left._rope, left._start, left.Length + right.Length, left._zeroCount + rz, left._onesCount + ro);
            }

            // Create a copy of the rope to extend
            var rope = left._rope.CloneSlice(left._start, left.Length, left.Length + right.Length);
            rope.Append(right);
            return new RopeSlice(rope, 0, rope.Length, left._zeroCount + rz, left._onesCount + ro);
        }

        public static RopeSlice Concat(in ReadOnlySpan<char> left, in RopeSlice right)
        {
            // If part of the concat is an empty string return the other half.
            if (left.Length == 0)
                return right;

            var (lz, lo) = left.CountDigits();
            if (right._rope == null)
                return new RopeSlice(new Rope(left), 0, left.Length, lz, lo);

            // Create a new rope containing the data
            var rope = new Rope(left.Length + right.Length);
            rope.Append(left);
            rope.Append(right.AsSpanUnchecked);
            return new RopeSlice(rope, 0, rope.Length, lz + right._zeroCount, lo + right._onesCount);
        }

        public static RopeSlice Concat(in RopeSlice left, in char right)
        {
            unsafe
            {
                var r = right;
                return Concat(left, new Span<char>(&r, 1));
            }
        }
        #endregion

        #region remove
        public static RopeSlice Remove(in RopeSlice haystack, in RopeSlice needle)
        {
            return Remove(haystack, needle.AsSpan, needle._zeroCount, needle._onesCount);
        }

        public static RopeSlice Remove(in RopeSlice haystack, in ReadOnlySpan<char> needle)
        {
            // Check trivial cases
            if (needle.Length == 0 || haystack.Length < needle.Length)
                return haystack;

            // Special case for removing booleans from the string
            if (needle.Length == 1)
            {
                if (needle[0] == '0' && haystack._zeroCount.IsZero)
                    return haystack;
                if (needle[0] == '1' && haystack._onesCount.IsZero)
                    return haystack;
            }

            // We just checked the length of both things is not zero. Which means the rope cannot be null either.
            Debug.Assert(haystack._rope != null);

            // Count the ones/zeroes in the needle, we'll need this anyway if the needle is found in the haystack
            var (needleZeros, needleOnes) = needle.CountDigits();

            return Remove(haystack, needle, needleZeros, needleOnes);
        }

        private static RopeSlice Remove(in RopeSlice haystack, in ReadOnlySpan<char> needle, SaturatingByte needleZeros, SaturatingByte needleOnes)
        {
            // Any empty needle is a no-op
            if (needle.Length == 0 || haystack.Length == 0)
                return haystack;

            // Special case for removing booleans from the string
            if (needle.Length == 1)
            {
                if (needle[0] == '0' && haystack._zeroCount.IsZero)
                    return haystack;
                if (needle[0] == '1' && haystack._onesCount.IsZero)
                    return haystack;
            }

            // We just checked the length of both things is not zero. Which means the rope cannot be null either.
            Debug.Assert(haystack._rope != null);

            // Find the needle slice within the haystack
            var index = haystack.LastIndexOf(needle, needleZeros, needleOnes);
            if (index < 0)
                return haystack;

            return Remove(in haystack, index, needle.Length, (needleZeros, needleOnes));
        }

        public static RopeSlice Remove(in RopeSlice haystack, char right)
        {
            unsafe
            {
                var r = right;
                return Remove(haystack, new Span<char>(&r, 1));
            }
        }

        private static RopeSlice Remove(in RopeSlice haystack, int startIndex, int needleLength, (SaturatingByte zero, SaturatingByte ones) removed)
        {
            Debug.Assert(haystack._rope != null);

            // Calculate the counts for the new string
            var zeroCount = haystack._zeroCount - removed.zero;
            var onesCount = haystack._onesCount - removed.ones;
            
            // If left slice _starts_ with the right string we can just offset the start of the slice
            if (startIndex == 0)
            {
                var start = haystack._start + needleLength;
                var length = haystack.Length - needleLength;
                if (length == 0)
                    start = 0;
                return new RopeSlice(haystack._rope, start, length, haystack._zeroCount - removed.zero, haystack._onesCount - removed.ones);
            }

            // If the left slice _ends_ with the right string we can just shorten the slice
            if (startIndex + needleLength == haystack.Length)
                return new RopeSlice(haystack._rope, haystack._start, haystack.Length - needleLength, zeroCount, onesCount);

            // Make a new rope out of the two parts
            var leftPart = new Slice(haystack._rope, haystack._start, startIndex);
            var rightPart = new Slice(haystack._rope, haystack._start + startIndex + needleLength, haystack.Length - needleLength - startIndex);
            var concat = new Rope(leftPart, rightPart);
            return new RopeSlice(concat, 0, concat.Length, zeroCount, onesCount);
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

            return LastIndexOf(needle.AsSpanUnchecked, needle._zeroCount, needle._onesCount);
        }

        private int LastIndexOf(in ReadOnlySpan<char> needle, SaturatingByte needleZeroes, SaturatingByte needleOnes)
        {
            Debug.Assert(needle.Length > 0);

            // if haystack is shorter than needle then haystack can't possibly contain needle
            if (Length < needle.Length)
                return -1;

            // If the needle has more zeroes than the haystack it can't possibly be in the haystack
            // Skip this check if either counter is saturated
            if (!_zeroCount.IsSaturated && !needleZeroes.IsSaturated && _zeroCount < needleZeroes)
                return -1;

            // If the needle has more ones than the haystack it can't possibly be in the haystack
            // Skip this check if either counter is saturated
            if (!_onesCount.IsSaturated && !needleOnes.IsSaturated && _onesCount < needleOnes)
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

            var zc = _zeroCount;
            var oc = _onesCount;
            var lastChar = LastCharUnchecked();
            if (lastChar == '0')
                zc--;
            else if (lastChar == '1')
                oc--;

            return new RopeSlice(_rope, _start, Length - 1, zc, oc);
        }

        public RopeSlice Trim(int length)
        {
            if (Length <= length || _rope == null)
                return this;

            // Keep track of the slice that's been removed
            var removedStart = length;
            var removedEnd = Length;

            // Count the number of removed characters
            var lostZero = 0;
            var lostOnes = 0;

            // Create a slice large enough to hold some chars without blowing up the stack
            Span<char> chars = stackalloc char[128];

            // Get chunks of the removed section and count the ones/zeroes
            while (removedStart < removedEnd)
            {
                var count = Math.Min(chars.Length, removedEnd - removedStart);
                _rope.CopySlice(removedStart, count, chars);
                for (int i = 0; i < count; i++)
                {
                    var c = chars[i];
                    if (c == '0')
                        lostZero++;
                    else if (c == '1')
                        lostOnes++;
                }

                removedStart += count;
            }

            return new RopeSlice(_rope, _start, length, _zeroCount - lostZero, _onesCount - lostOnes);
        }
    }
}

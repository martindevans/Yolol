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
        private char[]? _chars;

        private Slice _left;
        private Slice _right;
        #endregion

        public int Length => _variant == Variant.Flat ? _count : _left.Length + _right.Length;

        public char this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                Flatten();
                Debug.Assert(_chars != null);
                return _chars[index];
            }
        }

        #region constructors
        public Rope(int capacity = 64)
        {
            _chars = new char[64];
            _variant = Variant.Flat;
        }

        public Rope(string initial, int capacity = 64)
        {
            _chars = new char[Math.Max(initial.Length, capacity)];
            _variant = Variant.Flat;

            initial.CopyTo(0, _chars, 0, initial.Length);
            _count = initial.Length;
        }

        public Rope(Slice left, Slice right)
        {
            _left = left;
            _right = right;
            _variant = Variant.Concat;

            _chars = null;
            _count = 0;
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

            _left.AsSpan.CopyTo(_chars.AsSpan());
            _right.AsSpan.CopyTo(_chars.AsSpan(_left.Length));
            _count = _left.Length + _right.Length;

            _left = default;
            _right = default;
        }

        public string ToString(int start, int length)
        {
            Flatten();
            Debug.Assert(_chars != null);
            return new string(_chars.AsSpan(start, length));
        }

        public void Append(ReadOnlySpan<char> other)
        {
            Flatten();
            Debug.Assert(_chars != null);

            if (_count + other.Length > _chars.Length)
            {
                var expanded = new char[Math.Max(_chars.Length * 2, _count + other.Length)];
                _chars.CopyTo(expanded, 0);
                _chars = expanded;
            }

            other.CopyTo(_chars.AsSpan(_count));
            _count += other.Length;
        }

        public Rope CloneSlice(int start, int length, int capacity)
        {
            Flatten();
            Debug.Assert(_chars != null);

            var r = new Rope(Math.Max(length, capacity));
            r.Append(_chars.AsSpan(start, length));
            return r;
        }

        public ReadOnlySpan<char> GetSpan(int sourceIndex, int sourceCount)
        {
            Flatten();
            Debug.Assert(_chars != null);

            //ncrunch: no coverage start
            if (sourceCount - sourceIndex > Length)
                throw new ArgumentOutOfRangeException(nameof(sourceCount), "(sourceCount - sourceIndex) is longer than source");
            //ncrunch: no coverage end

            return _chars.AsSpan(sourceIndex, sourceCount);
        }
    }

    internal readonly struct Slice
    {
        public readonly Rope Rope;
        public readonly int Start;
        public readonly int Length;

        public Slice(Rope rope, int start, int length)
        {
            Rope = rope;
            Start = start;
            Length = length;
        }

        public ReadOnlySpan<char> AsSpan => Rope.GetSpan(Start, Length);
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
                _rope = rope.CloneSlice(start, length, length);
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

        public bool Equals(string other)
        {
            return AsSpan.SequenceEqual(other);
        }

        public override int GetHashCode()
        {
            HashCode c = new HashCode();

            var s = AsSpan;
            for (int i = 0; i < s.Length; i++)
                c.Add(s[i]);

            return c.ToHashCode();
        }

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

        public RopeSlice PopLast()
        {
            if (_rope == null)
                throw new InvalidOperationException("Attempted to `PopLast` on null `RopeSlice`");
            if (Length < 1)
                throw new InvalidOperationException("Attempted to `PopLast` on empty `RopeSlice`");

            var zc = default(SaturatingByte);
            var oc = default(SaturatingByte);
            var last = this[Length - 1];
            if (last == '0')
                zc = new SaturatingByte(1);
            else if (last == '1')
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

            // If the end of the left span points to the end of the underlying rope then
            // the rope can be extended in place.
            if (left._start + left.Length == left._rope.Length)
            {
                left._rope.Append(right.AsSpanUnchecked);
                return new RopeSlice(
                    left._rope,
                    left._start, left.Length + right.Length,
                    left._zeroCount + right._zeroCount,
                    left._onesCount + right._onesCount
                );
            }

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
            {
                var r = new Rope(left.Length);
                r.Append(left);
                return new RopeSlice(r, 0, r.Length, lz, lo);
            }

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

        private int LastIndexOf(in RopeSlice needle)
        {
            if (needle.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(needle), "Length of needle must be > 0");
            Debug.Assert(needle._rope != null);

            // if this is shorter than needle then this can't possibly contain needle
            if (Length < needle.Length)
                return -1;

            return LastIndexOf(needle.AsSpanUnchecked, needle._zeroCount, needle._onesCount);
        }

        private int LastIndexOf(in ReadOnlySpan<char> needle, SaturatingByte needleZeroes, SaturatingByte needleOnes)
        {
            if (needle.Length <= 0)
                throw new ArgumentOutOfRangeException(nameof(needle), "Length of needle must be > 0");

            // if this is shorter than needle then this can't possibly contain needle
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

        public RopeSlice Decrement()
        {
            if (Length == 0)
                throw new InvalidOperationException("Cannot decrement empty slice");
            Debug.Assert(_rope != null);

            var zc = _zeroCount;
            var oc = _onesCount;
            var last = this[Length - 1];
            if (last == '0')
                zc--;
            else if (last == '1')
                oc--;

            return new RopeSlice(_rope, _start, Length - 1, zc, oc);
        }

        public RopeSlice Trim(int length)
        {
            if (Length <= length || _rope == null)
                return this;

            // Count how many zeroes and ones have been removed
            var lostZero = 0;
            var lostOnes = 0;
            for (var i = length; i < Length; i++)
            {
                var c = this[i];
                if (c == '0')
                    lostZero++;
                else if (c == '1')
                    lostOnes++;
            }

            return new RopeSlice(_rope, _start, length, _zeroCount - lostZero, _onesCount - lostOnes);
        }
    }
}

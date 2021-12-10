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

    //todo:add StructLayout/FieldOffset back in once Blazor WASM is fixed (https://github.com/dotnet/runtime/issues/61385)
    //[StructLayout(LayoutKind.Explicit)]
    internal readonly struct RopeSlice
        : IEquatable<string>
    {
        //[FieldOffset(0)]
        private readonly ushort _start;

        // Keep count of the total number of zeroes/ones in the string. This can be used to accelerate
        // the very common task of subtracting booleans from the string.
        //[FieldOffset(2)]
        private readonly SaturatingCounters _counts;

        //[field: FieldOffset(4)]
        public int Length { get; }

        //[FieldOffset(8)]
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

        private Slice AsSlice => new Slice(_rope!, _start, Length);

        #region constructors
        private RopeSlice(Rope rope, int start, int length, SaturatingCounters counts)
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
            _counts = counts;

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

            Debug.Assert(CountInSpan(0, Length).Equals(counts));
        }

        public RopeSlice(string str)
            : this(str.AsMemory().Span)
        {
        }

        public RopeSlice(ReadOnlySpan<char> str)
        {
            if (str.Length == 0)
            {
                _rope = null;
                _start = 0;
                Length = 0;

                _counts = default;
            }
            else
            {
                _rope = new Rope(str);
                _start = 0;
                Length = str.Length;

                _counts = str.CountDigits();
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

        #region substring
        private RopeSlice Substring(ushort substringStart, int substringLength)
        {
            if (substringLength == 0)
                return default;
            if (substringLength == Length)
                return this;

            Debug.Assert(_rope != null);
            Debug.Assert(substringStart + substringLength <= Length);

            var counts = CountInSpan(substringStart, substringLength);
            return new RopeSlice(_rope, _start + substringStart, substringLength, counts);
        }

        private RopeSlice Substring(int substringStart, int substringLength, SaturatingCounters removed)
        {
            if (substringLength == 0)
                return default;
            if (substringLength == Length)
                return this;

            Debug.Assert(_rope != null);
            Debug.Assert(substringStart + substringLength <= Length);

            var counts = _counts - removed;
            return new RopeSlice(_rope, _start + substringStart, substringLength, counts);
        }

        /// <summary>
        /// Directly count one/zero characters in the given span of this slice
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private SaturatingCounters SimpleCountInSpan(ushort start, int length)
        {
            if (_rope == null)
                return default;

            // Keep track of the slice that's been removed
            var spanStart = _start + start;
            var spanEnd = _start + length;

            // Create a slice large enough to hold some chars without blowing up the stack
            Span<char> chars = stackalloc char[128];

            // Get chunks of the removed section and count the ones/zeroes
            var zeros = 0;
            var ones = 0;
            while (spanStart < spanEnd)
            {
                var count = Math.Min(chars.Length, spanEnd - spanStart);
                _rope.CopySlice(spanStart, count, chars);
                for (int i = 0; i < count; i++)
                {
                    var c = chars[i];
                    if (c == '0')
                        zeros++;
                    else if (c == '1')
                        ones++;
                }

                spanStart += count;
            }

            var counts = new SaturatingCounters(new SaturatingByte(zeros), new SaturatingByte(ones));

#if DEBUG
            Debug.Assert(AsSpan[start..(start + length)].CountDigits().Equals(counts));
#endif

            return counts;
        }

        /// <summary>
        /// Count how many ones/zeroes are in the given span
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private SaturatingCounters CountInSpan(ushort start, int length)
        {
            //todo: improve this to count whichever is shorter:
            // - the given span
            // - everything else
            return SimpleCountInSpan(start, length);
        }
        #endregion

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

        public RopeSlice PopLast()
        {
            if (Length < 1)
                throw new InvalidOperationException("Attempted to `PopLast` on empty `RopeSlice`");
            Debug.Assert(_rope != null);

            return Substring((ushort)(Length - 1), 1);
        }

        #region concat
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RopeSlice Concat(RopeSlice left, RopeSlice right, int maxLength)
        {
            // If either part of the concat is an empty string (represented by a null rope) return the other half.
            if (left._rope == null || left.Length == 0)
                return right;
            if (right._rope == null || right.Length == 0 || left.Length >= maxLength)
                return left;

            // Create slices for the right part, respecting the length limit
            right = right.Substring(0, Math.Min(right.Length, maxLength - left.Length));

            // Create a new rope concatenating the two slices
            var rope = new Rope(left.AsSlice, right.AsSlice);
            return new RopeSlice(
                rope,
                0,
                rope.Length,
                left._counts + right._counts
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
            var rightRopeSlice = new RopeSlice(right);

            // If left is null just return right
            if (left._rope == null)
                return rightRopeSlice;

            return Concat(left, rightRopeSlice, maxLength);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RopeSlice Concat(ReadOnlySpan<char> left, in RopeSlice right, int maxLength)
        {
            // If part of the concat is an empty string return the other half.
            if (left.Length == 0)
                return right;

            // Trim left down to length
            left = left[..Math.Min(left.Length, maxLength)];
            var leftRopeSlice = new RopeSlice(left);

            // If the right is zero length or the left takes up the max string length just return left
            if (right.Length == 0 || left.Length == maxLength)
                return leftRopeSlice;

            // Take a slice of the right side, as much as possible without going over the max size
            var rightRopeSlice = right.Substring(0, Math.Min(right.Length, maxLength - left.Length));

            // Concat the two sides
            return Concat(leftRopeSlice, rightRopeSlice, maxLength);
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
        private static bool IsSubstringSearchPossible(SaturatingCounters haystack, int haystackLength, SaturatingCounters needle, int needleLength)
        {
            // Check if the needle has more zeroes/ones than the haystack.
            // If so, early exit immediately since the needle can't possibly be in the haystack.
            if (!needle.ZeroCount.IsSaturated && !haystack.ZeroCount.IsSaturated && needle.ZeroCount > haystack.ZeroCount)
                return false;
            if (!needle.OnesCount.IsSaturated && !haystack.OnesCount.IsSaturated && needle.OnesCount > haystack.OnesCount)
                return false;

            // Any empty needle is a no-op. An empty haystack can't contain anything
            if (needleLength == 0 || haystackLength == 0)
                return false;

            return true;
        }

        public static RopeSlice Remove(in RopeSlice haystack, in RopeSlice needle)
        {
            // Check if the needle has more zeroes/ones than the haystack.
            // If so, early exit immediately since the needle can't possibly be in the haystack.
            if (!IsSubstringSearchPossible(haystack._counts, haystack.Length, needle._counts, needle.Length))
                return haystack;

            // Find the needle slice within the haystack
            var index = haystack.LastIndexOf(needle);
            if (index < 0)
                return haystack;

            return Remove(in haystack, index, needle.Length, needle._counts);
        }

        public static RopeSlice Remove(in RopeSlice haystack, in ReadOnlySpan<char> needle)
        {
            var needleCounts = needle.CountDigits();
            if (!IsSubstringSearchPossible(haystack._counts, haystack.Length, needleCounts, needle.Length))
                return haystack;

            // We just checked the length of both things is not zero. Which means the rope cannot be null either.
            Debug.Assert(haystack._rope != null);

            // Find the needle slice within the haystack
            var index = haystack.LastIndexOf(needle);
            if (index < 0)
                return haystack;

            return Remove(in haystack, index, needle.Length, needleCounts);
        }

        public static RopeSlice Remove(in RopeSlice haystack, bool right)
        {
            // If there are no ones/zeroes in the string there's no need to search for it
            if (right && haystack._counts.OnesCount.IsZero)
                return haystack;
            if (!right && haystack._counts.ZeroCount.IsZero)
                return haystack;

            unsafe
            {
                var r = right ? '1' : '0';
                return Remove(haystack, new Span<char>(&r, 1));
            }
        }

        private static RopeSlice Remove(in RopeSlice haystack, int needleStartIndex, int needleLength, SaturatingCounters needleCounts)
        {
            Debug.Assert(haystack._rope != null);

            // If haystack _starts_ with the needle we can just offset the start of the slice
            if (needleStartIndex == 0)
            {
                var start = needleLength;
                var length = haystack.Length - needleLength;
                return haystack.Substring(start, length, removed: needleCounts);
            }

            // If the left slice _ends_ with the right string we can just shorten the slice
            if (needleStartIndex + needleLength == haystack.Length)
                return haystack.Substring(0, haystack.Length - needleLength, removed: needleCounts);

            // Make a new rope out of the two parts
            var leftPart = new Slice(haystack._rope, haystack._start, needleStartIndex);
            var rightPart = new Slice(haystack._rope, haystack._start + needleStartIndex + needleLength, haystack.Length - needleLength - needleStartIndex);
            var concat = new Rope(leftPart, rightPart);
            return new RopeSlice(concat, 0, concat.Length, haystack._counts - needleCounts);
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

            return Substring(0, Length - 1);
        }

        public RopeSlice Trim(int length)
        {
            if (Length <= length || _rope == null)
                return this;

            return Substring(0, length);
        }
    }
}

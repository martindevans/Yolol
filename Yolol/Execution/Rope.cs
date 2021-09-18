using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Yolol.Execution.Extensions;

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
            var r = new Rope(Math.Max(length, capacity));
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

            var zc = default(SaturatingByte);
            var oc = default(SaturatingByte);
            var last = this[Length - 1];
            if (last == '0')
                zc = new SaturatingByte(1);
            else if (last == '1')
                oc = new SaturatingByte(1);

            return new RopeSlice(_rope, _start + Length - 1, 1, zc, oc);
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
                return new RopeSlice(
                    left._rope,
                    left._start, left.Length + right.Length,
                    left._zeroCount + right._zeroCount,
                    left._onesCount + right._onesCount
                );
            }

            // Create a copy of the rope to extend
            var rope = left._rope.CloneSlice(left._start, left.Length, left.Length + right.Length);
            rope.Append(right._rope, right._start, right.Length);
            return new RopeSlice(
                rope,
                0,
                rope.Length,
                left._zeroCount + right._zeroCount,
                left._onesCount + right._onesCount
            );
        }

        public static RopeSlice Concat(in RopeSlice left, in ReadOnlySpan<char> right)
        {
            // If either part of the concat is an empty string return the other half.
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
            // If either part of the concat is an empty string return the other half.
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
            rope.Append(right._rope, right._start, right.Length);
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

        public static RopeSlice Remove(in RopeSlice haystack, in RopeSlice needle)
        {
            // if the needle or haystack is an empty string early exit
            if (needle.Length == 0 || haystack.Length == 0)
                return haystack;

            // We just checked the length of both things is not zero. Which means the rope cannot be null either.
            Debug.Assert(haystack._rope != null);
            Debug.Assert(needle._rope != null);

            // Find the right slice within the left slice
            var index = haystack.LastIndexOf(needle);
            if (index < 0)
                return haystack;

            return Remove(in haystack, index, needle.Length, (needle._zeroCount, needle._onesCount));
        }

        public static RopeSlice Remove(in RopeSlice haystack, in ReadOnlySpan<char> needle)
        {
            if (needle.Length == 0 || haystack.Length == 0)
                return haystack;

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
            {
                return new RopeSlice(haystack._rope, haystack._start, haystack.Length - needleLength, zeroCount, onesCount);
            }

            // We'll have to make a new rope and remove it
            var rope = new Rope(haystack.Length);

            // Copy bit of haystack to the left of the needle into new rope
            rope.Append(haystack._rope, haystack._start, startIndex);

            // Copy bit of haystack to the right of the needle into new rope
            rope.Append(haystack._rope, haystack._start + startIndex + needleLength, haystack.Length - (startIndex + needleLength));

            return new RopeSlice(rope, 0, rope.Length, zeroCount, onesCount);
        }

        private int LastIndexOf(in RopeSlice needle)
        {
            if (needle.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(needle), "Length of needle must be > 0");
            Debug.Assert(needle._rope != null);

            // if this is shorter than needle then this can't possibly contain needle
            if (Length < needle.Length)
                return -1;

            // Copy the strings to two stack buffers and search in those buffers.
            unsafe
            {
                var needleBuffer = stackalloc char[needle.Length];
                var needleSpan = new Span<char>(needleBuffer, needle.Length);
                needle._rope.CopyTo(needleSpan, needle._start, needle.Length);

                return LastIndexOf(needleSpan, needle._zeroCount, needle._onesCount);
            }
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

            // Copy the strings to two stack buffers and search in those buffers.
            unsafe
            {
                var haystackBuffer = stackalloc char[Length];
                var haystackSpan = new Span<char>(haystackBuffer, Length);
                _rope.CopyTo(haystackSpan, _start, Length);

                return haystackSpan.LastIndexOf(needle);
            }
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

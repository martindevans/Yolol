using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Yolol.Execution.Attributes;

namespace Yolol.Execution
{
    //todo:add StructLayout/FieldOffset back in once Blazor WASM is fixed (https://github.com/dotnet/runtime/issues/61385)
    //[StructLayout(LayoutKind.Explicit)]
    public readonly struct Number
        : IEquatable<Number>, IComparable<Number>
    {
        public const int Scale = 1000;
        public const int Decimals = 3;
        private const float Pi = 3.14159265359f;

        public static readonly Number MinValue = new Number(long.MinValue);
        public static readonly Number MaxValue = new Number(long.MaxValue);
        public static readonly Number One = new Number(1000);
        public static readonly Number Zero = new Number(0);

        //[FieldOffset(0)]
        private readonly long _value;

        /// <summary>
        /// Fetches the raw underlying value which represents this number. This Can be converted back into a `Number` by using `Number.FromRaw`
        /// </summary>
        public long RawValue => _value;

        private Number(long num)
        {
            _value = num;
        }

        /// <summary>
        /// Create a number from a raw value (fixedpoint value with 3dp)
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static Number FromRaw(long num)
        {
            return new Number(num);
        }

        internal Span<char> ToString(Span<char> buffer)
        {
            var big = _value / Scale;
            var little = Math.Abs(_value - big * Scale);

            if (little == 0)
            {
                if (!big.TryFormat(buffer, out var written))
                    throw new InvalidOperationException($"Attempted to format a number with more than {buffer.Length} digits");
                return buffer[..written];
            }

            var bufferSize = buffer.Length;

            // Write out the "big" numbers
            int bigWritten;
            if (big == 0)
            {
                if (_value < 0)
                {
                    buffer[0] = '-';
                    buffer[1] = '0';
                    bigWritten = 2;
                }
                else
                {
                    buffer[0] = '0';
                    bigWritten = 1;
                }
            }
            else
            {
                if (!big.TryFormat(buffer, out bigWritten))
                    throw new InvalidOperationException($"Attempted to format a number with more than {bufferSize} digits");
            }

            // Write out a `.` character
            buffer[bigWritten] = '.';

            // Write out the "little" numbers
            if (!little.TryFormat(buffer.Slice(bigWritten + 1, bufferSize - bigWritten - 1), out var littleWritten, "D3"))
                throw new InvalidOperationException($"Attempted to format a number with more than {bufferSize} digits");

            // There may be trailing zeros after the "little" number, find them and shorten the `littleWritten` to match
            for (var i = bigWritten + 1 + littleWritten - 1; i >= 0; i--)
            {
                if (buffer[i] == '0')
                    littleWritten--;
                else
                    break;
            }

            return buffer[..(bigWritten + 1 + littleWritten)];
        }

        public override string ToString()
        {
            unsafe
            {
                const int bufferSize = 128;
                var buffer = stackalloc char[bufferSize];
                var span = ToString(new Span<char>(buffer, bufferSize));

                // Copy the characters out of the stack buffer into a heap allocated string
                return new string(buffer, 0, span.Length);
            }
        }

        public bool Equals(Number other)
        {
            return _value == other._value;
        }

        public override bool Equals(object? obj)
        {
            return obj is Number other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_value);
        }

        public int CompareTo(Number other)
        {
            return _value.CompareTo(other._value);
        }

        public static Number Parse(string s)
        {
            // First check if the number is out of the valid range
            var d = double.Parse(s, CultureInfo.InvariantCulture);
            if (d >= MaxValue._value)
                return MaxValue;
            else if (d <= MinValue._value)
                return MinValue;

            return (Number)decimal.Parse(s, CultureInfo.InvariantCulture);
        }

        #region casts
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Number(bool b)
        {
            return b ? One : Zero;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Number(int i)
        {
            return new Number(i * (long)Scale);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Number(double i)
        {
            if (double.IsPositiveInfinity(i) || double.IsNegativeInfinity(i) || double.IsNaN(i))
                return MinValue;

            var epsilon = i < 0 ? -0.00005 : 0.00005; 
            var n = (i + epsilon) * Scale;

            if (n > MaxValue._value || n < MinValue._value)
                return MinValue;

            return new Number((long)n);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Number(decimal d)
        {
            var n = d * Scale;
            if (n > MaxValue._value)
                return MaxValue;
            if (n < MinValue._value)
                return MinValue;

            return new Number((long)(d * Scale));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator decimal(Number n)
        {
            return ((decimal)n._value) / Scale;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator int(Number n)
        {
            return (int)(n._value / Scale);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator float(Number n)
        {
            return ((float)n._value) / Scale;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator double(Number n)
        {
            return ((double)n._value) / Scale;
        }
        #endregion

        #region mod
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool WillModThrow([IgnoreParam] Number _, Number r)
        {
            return r._value == 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool WillModThrow([IgnoreParam] Number _, Value r)
        {
            return r.Type != Type.Number || r.UnsafeNumber._value == 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool WillModThrow([IgnoreParam] Number _, bool r)
        {
            return !r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Number UnsafeMod(Number l, Number r)
        {
            if (l._value == long.MinValue && r._value == -1)
                return Zero;

            return new Number(l._value % r._value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Number UnsafeMod(Number l, Value r)
        {
            return UnsafeMod(l, r.Number);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Number UnsafeMod(Number l, bool _)
        {
            // An unsafe mod (i.e. guaranteed not to throw given the arguments) by a
            // bool must always be mod 1 because mod 0 would throw.

            return UnsafeMod(l, One);
        }

        [ErrorMetadata(nameof(WillModThrow), nameof(UnsafeMod))]
        public static Number operator %(Number l, Number r)
        {
            if (r._value == 0)
                throw new ExecutionException("Modulus by zero");

            return UnsafeMod(l, r);
        }

        public static StaticError operator %(Number _, YString __)
        {
            return new StaticError("Attempted to modulus by a string");
        }

        [ErrorMetadata(nameof(WillModThrow), nameof(UnsafeMod))]
        public static Value operator %(Number l, [TypeImplication(Type.Number)] Value r)
        {
            if (r.Type == Type.String)
                return new StaticError("Attempted to modulus by a string");
            else
                return UnsafeMod(l, r);
        }

        [ErrorMetadata(nameof(WillModThrow), nameof(UnsafeMod))]
        public static Number operator %(Number l, bool r)
        {
            return l % (Number)r;
        }
        #endregion

        #region multiply
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool WillMulThrow([IgnoreParam] Number _, Value v)
        {
            return v.Type == Type.String;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Number UnsafeMul(Number l, Value v)
        {
            return l * v.UnsafeNumber;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Number operator *(Number l, Number r)
        {
            return new Number((l._value * r._value) / Scale);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StaticError operator *(Number _, YString __)
        {
            return new StaticError("Attempted to multiply by a string");
        }

        [ErrorMetadata(nameof(WillMulThrow), nameof(UnsafeMul))]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Value operator *(Number l, [TypeImplication(Type.Number)] Value r)
        {
            if (r.Type != Type.Number)
                return new StaticError("Attempted to multiply by a string");
            else
                return l * r.UnsafeNumber;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Number operator *(Number l, bool r)
        {
            // Due to over/underflow `x*1` does not always equal `x`
            if (r)
                return l * One;
            else
                return Zero;
        }
        #endregion

        #region divide
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool WillDivThrow([IgnoreParam] Number _, Value r)
        {
            if (r.Type != Type.Number)
                return true;
            return r.UnsafeNumber._value == 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool WillDivThrow([IgnoreParam] Number _, Number r)
        {
            return r._value == 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool WillDivThrow([IgnoreParam] Number _, bool r)
        {
            return !r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Number UnsafeDivide(Number l, Number r)
        {
            return new Number(l._value * Scale / r._value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Number UnsafeDivide(Number l, Value r)
        {
            return UnsafeDivide(l, r.UnsafeNumber);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Number UnsafeDivide(Number l, bool _)
        {
            // Due to over/underflow `x/1` does not always equal `x`
            return UnsafeDivide(l, One);
        }

        [ErrorMetadata(nameof(WillDivThrow), nameof(UnsafeDivide))]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Number operator /(Number l, Number r)
        {
            if (r._value == 0)
                throw new ExecutionException("Divide by zero");

            return UnsafeDivide(l, r);
        }

        public static StaticError operator /(Number _, YString __)
        {
            return new StaticError("Attempted to divide by a string");
        }

        [ErrorMetadata(nameof(WillDivThrow), nameof(UnsafeDivide))]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Value operator /(Number l, [TypeImplication(Type.Number)] Value r)
        {
            if (r.Type != Type.Number)
                return new StaticError("Attempted to divide by a string");
            
            return l / r.UnsafeNumber;
        }

        [ErrorMetadata(nameof(WillDivThrow), nameof(UnsafeDivide))]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Number operator /(Number l, bool r)
        {
            if (r)
                return l;
            
            throw new ExecutionException("Divide by zero");
        }
        #endregion

        #region add
        public static Number operator +(Number l, Number r)
        {
            return new Number(l._value + r._value);
        }

        public static YString operator +(Number l, YString r)
        {
            unsafe
            {
                const int bufferSize = 128;
                var buffer = stackalloc char[bufferSize];
                var leftSpan = l.ToString(new Span<char>(buffer, bufferSize));

                return leftSpan + r;
            }
        }

        public static Value operator +(Number l, Value r)
        {
            return (Value)l + r;
        }

        public static Number operator +(Number l, bool r)
        {
            return l + (Number)r;
        }
        #endregion

        #region subtract
        public static Number operator -(Number l, Number r)
        {
            return new Number(l._value - r._value);
        }

        public static YString operator -(Number l, YString r)
        {
            return new YString(l.ToString()) - r;
        }

        public static Value operator -(Number l, Value r)
        {
            return (Value)l - r;
        }

        public static Number operator -(Number l, bool r)
        {
            return l - (Number)r;
        }
        #endregion

        #region negate
        public static Number operator -(Number n)
        {
            return new Number(-n._value);
        }
        #endregion

        #region >
        public static bool operator >(Number l, Number r)
        {
            return l._value > r._value;
        }

        public static bool operator >(Number l, YString r)
        {
            return r < l;
        }

        public static bool operator >(Number l, Value r)
        {
            if (r.Type == Type.Number)
                return l > r.Number;
            else
                return l > r.String;
        }

        public static bool operator >(Number l, bool r)
        {
            return l > (Number)r;
        }
        #endregion

        #region <
        public static bool operator <(Number l, Number r)
        {
            return l._value < r._value;
        }

        public static bool operator <(Number l, YString r)
        {
            return r > l;
        }

        public static bool operator <(Number l, Value r)
        {
            if (r.Type == Type.Number)
                return l < r.Number;
            else
                return l < r.String;
        }

        public static bool operator <(Number l, bool r)
        {
            return l < (Number)r;
        }
        #endregion

        #region >=
        public static bool operator >=(Number l, Number r)
        {
            return l._value >= r._value;
        }

        public static bool operator >=(Number l, YString r)
        {
            return r <= l;
        }

        public static bool operator >=(Number l, Value r)
        {
            if (r.Type == Type.Number)
                return l >= r.Number;
            else
                return l >= r.String;
        }

        public static bool operator >=(Number l, bool r)
        {
            return l >= (Number)r;
        }
        #endregion

        #region <=
        public static bool operator <=(Number l, Number r)
        {
            return l._value <= r._value;
        }

        public static bool operator <=(Number l, YString r)
        {
            return r >= l;
        }

        public static bool operator <=(Number l, Value r)
        {
            if (r.Type == Type.Number)
                return l <= r.Number;
            else
                return l <= r.String;
        }

        public static bool operator <=(Number l, bool r)
        {
            return l <= (Number)r;
        }
        #endregion

        #region ==
        public static bool operator ==(Number l, Number r)
        {
            return l._value == r._value;
        }

        public static bool operator ==(Number _, YString __)
        {
            return false;
        }

        public static bool operator ==(Number l, Value r)
        {
            if (r.Type == Type.Number)
                return l == r.Number;
            else
                return false;
        }

        public static bool operator ==(Number l, bool r)
        {
            return l == (Number)r;
        }
        #endregion

        #region !=
        public static bool operator !=(Number l, Number r)
        {
            return l._value != r._value;
        }

        public static bool operator !=(Number _, YString __)
        {
            return true;
        }

        public static bool operator !=(Number l, Value r)
        {
            if (r.Type == Type.Number)
                return l != r.Number;
            else
                return true;
        }

        public static bool operator !=(Number l, bool r)
        {
            return l != (Number)r;
        }
        #endregion

        public static Number operator ++(Number value)
        {
            return new Number(value._value + 1000);
        }

        public static Number operator --(Number value)
        {
            return new Number(value._value - 1000);
        }


        public Number Abs()
        {
            if (_value == long.MinValue)
                return MinValue;

            var abs = Math.Abs(_value);
            return new Number(abs);
        }

        public Number Sqrt()
        {
            if (_value < 0)
                return MinValue;
            if (_value >= 9223372036854775000)
                return MinValue;

            var converted = (double)this;
            var result = Math.Sqrt(converted);

            return (Number)result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float ToDegrees(float radians)
        {
            const float rad2Deg = 360f / ((float)Math.PI * 2);
            return radians * rad2Deg;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double ToRadians(float degrees)
        {
            return Pi * (degrees / 180.0f);
        }

        public Number Sin()
        {
            var r = ToRadians((float)this);
            var s = Math.Round(Math.Sin(r), 3);

            return (Number)s;
        }

        public Number Cos()
        {
            var r = ToRadians((float)this);
            var s = Math.Round(Math.Cos(r), 3);

            return (Number)s;
        }

        public Number Tan()
        {
            var rads = ToRadians((float)this);
            var i = (long)(Math.Tan(rads) * Scale);
            return FromRaw(i);
        }

        public Number ArcTan()
        {
            return (Number)ToDegrees((float)Math.Atan((float)this));
        }

        public Number ArcSin()
        {
            return (Number)ToDegrees((float)Math.Asin((float)this));
        }

        public Number ArcCos()
        {
            return (Number)ToDegrees((float)Math.Acos((float)this));
        }


        public Number Exponent(Value right)
        {
            if (right.Type == Type.Number)
                return Exponent(right.UnsafeNumber);
            else
                throw new ExecutionException("Attempted to exponent a string");
        }

#pragma warning disable CA1822 // Mark members as static
        public StaticError Exponent(YString _)
#pragma warning restore CA1822 // Mark members as static
        {
            return new StaticError("Attempted to exponent a string");
        }

        public Number Exponent(Number number)
        {
            var l = (double)this;
            var r = (double)number;
            var v = Math.Pow(l, r);

            return (Number)v;
        }

        public Number Exponent(bool right)
        {
            return Exponent((Number)right);
        }

        public Number Factorial()
        {
            if (_value < 0)
                return MinValue;
            if (_value > 63000)
                return Zero;

            var v = this;
            var i = 0;
            var result = 1L;
            while (v._value > 0 && result != 0)
            {
                i++;
                v--;

                result *= i;
            }

            return FromRaw(result * Scale);
        }
    }
}

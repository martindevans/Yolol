using System;
using Yolol.Execution.Attributes;

namespace Yolol.Execution
{
    public readonly struct Number
        : IEquatable<Number>
    {
        public const int Scale = 1000;
        public const int Decimals = 3;

        public static readonly Number MinValue = new Number(long.MinValue);
        public static readonly Number MaxValue = new Number(long.MaxValue);
        public static readonly Number One = new Number(1000);
        public static readonly Number Zero = new Number(0);

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
                return buffer.Slice(0, written);
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

            return buffer.Slice(0, bigWritten + 1 + littleWritten);
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

        public override bool Equals(object obj)
        {
            return obj is Number other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_value);
        }

        public static Number Parse(string s)
        {
            // First check if the number is out of the valid range
            var d = double.Parse(s);
            if (d >= MaxValue._value)
                return MaxValue;
            else if (d <= MinValue._value)
                return MinValue;

            return (Number)decimal.Parse(s);
        }

        public static explicit operator Number(bool b)
        {
            return b ? One : Zero;
        }

        public static explicit operator Number(int i)
        {
            return new Number(i * (long)Scale);
        }

        public static explicit operator Number(double i)
        {
            var n = i * Scale;
            if (n > MaxValue._value)
                return MaxValue;
            if (n < MinValue._value)
                return MinValue;

            return new Number((long)n);
        }

        public static explicit operator Number(decimal d)
        {
            var n = d * Scale;
            if (n > MaxValue._value)
                return MaxValue;
            if (n < MinValue._value)
                return MinValue;

            return new Number((long)(d * Scale));
        }

        public static explicit operator decimal(Number n)
        {
            return ((decimal)n._value) / Scale;
        }

        public static explicit operator int(Number n)
        {
            return (int)(n._value / Scale);
        }

        public static explicit operator float(Number n)
        {
            return ((float)n._value) / Scale;
        }


        internal static bool WillModThrow(Number l, Number r)
        {
            return r._value == 0;
        }

        internal static bool WillModThrow(Number _, Value r)
        {
            return r.Type == Type.String || r.Number == 0;
        }

        internal static bool WillModThrow(Number _, bool r)
        {
            return !r;
        }

        [ErrorMetadata(nameof(WillModThrow))]
        public static Number operator %(Number l, Number r)
        {
            if (r._value == 0)
                throw new ExecutionException("Modulus by zero");

            return new Number(l._value % r._value);
        }

        public static StaticError operator %(Number _, YString __)
        {
            return new StaticError("Attempted to modulus by a string");
        }

        [ErrorMetadata(nameof(WillModThrow))]
        public static Value operator %(Number l, Value r)
        {
            if (r.Type == Type.String)
                return new StaticError("Attempted to modulus by a string");
            else
                return l % r.Number;
        }

        [ErrorMetadata(nameof(WillModThrow))]
        public static Number operator %(Number l, bool r)
        {
            return l % (Number)r;
        }


        public static Number operator *(Number l, Number r)
        {
            return new Number((l._value * r._value) / Scale);
        }

        public static StaticError operator *(Number l, YString r)
        {
            return new StaticError("Attempted to multiply by a string");
        }

        public static Value operator *(Number l, Value r)
        {
            if (r.Type == Type.String)
                return new StaticError("Attempted to multiply by a string");
            else
                return l * r.Number;
        }

        public static Number operator *(Number l, bool r)
        {
            return l * (Number)r;
        }


        internal static bool WillDivThrow(Number _, Value r)
        {
            if (r.Type == Type.String)
                return true;
            return r.Number == 0;
        }

        internal static bool WillDivThrow(Number _, Number r)
        {
            return r == 0;
        }

        internal static bool WillDivThrow(Number _, bool r)
        {
            return !r;
        }

        [ErrorMetadata(nameof(WillDivThrow))]
        public static Number operator /(Number l, Number r)
        {
            if (r._value == 0)
                throw new ExecutionException("Divide by zero");

            return new Number((l._value * Scale) / r._value);
        }

        public static StaticError operator /(Number _, YString __)
        {
            return new StaticError("Attempted to divide by a string");
        }

        [ErrorMetadata(nameof(WillDivThrow))]
        public static Value operator /(Number l, Value r)
        {
            if (r.Type == Type.String)
                return new StaticError("Attempted to divide by a string");
            else
                return l / r.Number;
        }

        [ErrorMetadata(nameof(WillDivThrow))]
        public static Number operator /(Number l, bool r)
        {
            return l / (Number)r;
        }


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


        public static Number operator -(Number n)
        {
            return new Number(-n._value);
        }


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
            return new Number(Math.Abs(_value));
        }

        public Number Sqrt()
        {
            if (this < 0)
                throw new ExecutionException("Attempted to Sqrt a negative value");

            return (Number)(double)Math.Sqrt((float)this);
        }

        private static float ToDegrees(float radians)
        {
            const float rad2Deg = 360f / ((float)Math.PI * 2);
            return radians * rad2Deg;
        }

        private static float ToRadians(float degrees)
        {
            const float deg2Rad = (float)Math.PI * 2 / 360f;
            return degrees * deg2Rad;
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
            if (this == 90)
                return MaxValue;

            var r = ToRadians((float)this);
            var s = Math.Round(Math.Tan(r), 3);

            return (Number)s;
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
                return Exponent(right.Number);
            else
                throw new ExecutionException("Attempted to exponent a string");
        }

        public StaticError Exponent(YString _)
        {
            return new StaticError("Attempted to exponent a string");
        }

        public Number Exponent(Number number)
        {
            var v = Math.Pow((double)this, (double)number);

            if (double.IsPositiveInfinity(v))
                return MaxValue;

            if (double.IsNegativeInfinity(v) || double.IsNaN(v))
                return MinValue;

            return (Number)v;
        }

        public Number Exponent(bool right)
        {
            return Exponent((Number)right);
        }

        public Number Factorial()
        {
            var v = this;
            var i = 0;
            var result = 1;
            while (v > 0)
            {
                i++;
                v--;

                result *= i;
            }

            return (Number)result;
        }
    }
}

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Yolol.Execution.Attributes;
using Yolol.Grammar.AST.Expressions;

namespace Yolol.Execution
{
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct Value
        : IEquatable<Value>
    {
        [FieldOffset(0)]
        private readonly Number _number;
        public Number Number
        {
            get
            {
                if (_type != Type.Number)
                    throw new InvalidCastException($"Attempted to access value of type {_type} as a Number");
                return _number;
            }
        }

        [FieldOffset(0)]
        private readonly YString _string;
        public YString String
        {
            get
            {
                if (_type != Type.String)
                    throw new InvalidCastException($"Attempted to access value of type {_type} as a String");
                return _string;
            }
        }

        [FieldOffset(16)]
        private readonly Type _type;
        public Type Type => _type;

        internal Number UnsafeNumber => _number;
        internal YString UnsafeString => _string;

        public Value(string str)
        {
            // initialisation order is important because String and number fields overlap!
            // The number must be set before the string overwrites it!
            _number = default;
            _string = new YString(str);
            _type = Type.String;
        }

        public Value(YString str)
        {
            // initialisation order is important because String and number fields overlap!
            // The number must be set before the string overwrites it!
            _number = default;
            _string = str;
            _type = Type.String;
        }

        public Value(Number num)
        {
            // initialisation order is important because String and number fields overlap!
            // The string must be set before the number overwrites it!
            _string = default;
            _number = num;
            _type = Type.Number;
        }

        public Value(bool @bool)
            : this(@bool ? Number.One : Number.Zero)
        {
        }

        public static implicit operator Value(Number n)
        {
            return new Value(n);
        }

        public static explicit operator Value(decimal d)
        {
            return new Value((Number)d);
        }

        public static implicit operator Value(string s)
        {
            return new Value(s);
        }

        public override string ToString()
        {
            if (_type == Type.Number)
                return _number.ToString();
            else
                return _string.ToString();
        }

        private YString ToYString()
        {
            if (_type == Type.Number)
                return new YString(_number.ToString());
            else
                return _string;
        }

        public bool ToBool()
        {
            if (_type == Type.String)
                return false;
            else
                return _number != Number.Zero;
        }

        public object ToObject()
        {
            if (_type == Type.Number)
                return (decimal)_number;
            else
                return String.ToString();
        }

        public bool Equals(Value other)
        {
            if (_type == Type.Number && other._type == Type.Number)
                return _number == other._number;
            else if (_type == Type.String && other._type == Type.String)
                return _string == other._string;
            else
                return false;
        }

        public override bool Equals(object? obj)
        {
            return obj is Value other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)_type * 397;

                if (_type == Type.String)
                    hashCode *= _string.GetHashCode();
                else
                    hashCode *= _number.GetHashCode();

                return hashCode;
            }
        }

        #region op <
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(Value left, Value right)
        {
            return (left._type, right._type) switch {
                (Type.Number, Type.Number) => left._number < right._number,
                (Type.Number, Type.String) => left._number < right._string,
                (Type.String, Type.Number) => left._string < right._number,
                (Type.String, Type.String) => left._string < right._string,
                _ => throw new InvalidOperationException($"Cannot compare {left._type} < {right._type}")
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(Value left, YString right)
        {
            if (left._type == Type.Number)
                return left._number < right;
            else
                return left._string < right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(Value left, Number right)
        {
            if (left._type == Type.Number)
                return left._number < right;
            else
                return left._string < right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(Value left, bool right)
        {
            if (left._type == Type.Number)
                return left._number < right;
            else
                return left._string < right;
        }
        #endregion

        #region op <=
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(Value left, Value right)
        {
            return (left._type, right._type) switch {
                (Type.Number, Type.Number) => left._number <= right._number,
                (Type.Number, Type.String) => left._number <= right._string,
                (Type.String, Type.Number) => left._string <= right._number,
                (Type.String, Type.String) => left._string <= right._string,
                _ => throw new InvalidOperationException($"Cannot compare {left._type} <= {right._type}")
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(Value left, YString right)
        {
            if (left._type == Type.Number)
                return left._number <= right;
            else
                return left._string <= right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(Value left, Number right)
        {
            if (left._type == Type.Number)
                return left._number <= right;
            else
                return left._string <= right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(Value left, bool right)
        {
            if (left._type == Type.Number)
                return left._number <= right;
            else
                return left._string <= right;
        }
        #endregion

        #region op >
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(Value left, Value right)
        {
            return (left._type, right._type) switch {
                (Type.Number, Type.Number) => left._number > right._number,
                (Type.Number, Type.String) => left._number > right._string,
                (Type.String, Type.Number) => left._string > right._number,
                (Type.String, Type.String) => left._string > right._string,
                _ => throw new InvalidOperationException($"Cannot compare {left._type} > {right._type}")
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(Value left, YString right)
        {
            if (left._type == Type.Number)
                return left._number > right;
            else
                return left._string > right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(Value left, Number right)
        {
            if (left._type == Type.Number)
                return left._number > right;
            else
                return left._string > right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(Value left, bool right)
        {
            if (left._type == Type.Number)
                return left._number > right;
            else
                return left._string > right;
        }
        #endregion

        #region op >=
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(Value left, Value right)
        {
            return (left._type, right._type) switch {
                (Type.Number, Type.Number) => left._number >= right._number,
                (Type.Number, Type.String) => left._number >= right._string,
                (Type.String, Type.Number) => left._string >= right._number,
                (Type.String, Type.String) => left._string >= right._string,
                _ => throw new InvalidOperationException($"Cannot compare {left._type} >= {right._type}")
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(Value left, YString right)
        {
            if (left._type == Type.Number)
                return left._number >= right;
            else
                return left._string >= right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(Value left, Number right)
        {
            if (left._type == Type.Number)
                return left._number >= right;
            else
                return left._string >= right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(Value left, bool right)
        {
            if (left._type == Type.Number)
                return left._number >= right;
            else
                return left._string >= right;
        }
        #endregion

        #region op ==
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Value left, Value right)
        {
            return (left._type, right._type) switch {
                (Type.Number, Type.Number) => left._number == right._number,
                (Type.Number, Type.String) => left._number == right._string,
                (Type.String, Type.Number) => left._string == right._number,
                (Type.String, Type.String) => left._string == right._string,
                _ => throw new InvalidOperationException($"Cannot compare {left._type} == {right._type}")
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Value left, YString right)
        {
            if (left._type != Type.String)
                return false;
            else
                return left._string == right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Value left, Number right)
        {
            if (left._type != Type.Number)
                return false;
            else
                return left._number == right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Value left, bool right)
        {
            if (left._type != Type.Number)
                return false;
            else
                return left._number == right;
        }
        #endregion

        #region op !=
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Value left, Value right)
        {
            return !(left == right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Value left, YString right)
        {
            if (left._type != Type.String)
                return true;
            else
                return left._string != right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Value left, Number right)
        {
            if (left._type != Type.Number)
                return true;
            else
                return left._number != right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Value left, bool right)
        {
            if (left._type != Type.Number)
                return true;
            else
                return left._number != right;
        }
        #endregion

        #region op -
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Value operator -(Value left, Value right)
        {
            return (left._type, right._type) switch {
                (Type.Number, Type.Number) => left._number - right._number,
                (Type.Number, Type.String) => new Value(left._number - right._string),
                (Type.String, Type.Number) => new Value(left._string - right._number),
                (Type.String, Type.String) => new Value(left._string - right._string),
                _ => throw new InvalidOperationException($"Cannot execute {left._type} - {right._type}")
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YString operator -(Value left, YString right)
        {
            return left.ToYString() - right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Value operator -(Value l, Number r)
        {
            if (l._type == Type.Number)
                return l._number - r;
            else
                return new Value(l._string - r);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Value operator -(Value l, bool r)
        {
            return l - (Number)r;
        }
        #endregion

        #region op +
        public static Value Add(Value left, Value right, int maxStringLength)
        {
            return (left._type, right._type) switch
            {
                (Type.Number, Type.Number) => left._number + right._number,
                (Type.Number, Type.String) => new Value(YString.Add(left._number, right._string, maxStringLength)),
                (Type.String, Type.Number) => new Value(YString.Add(left._string, right._number, maxStringLength)),
                (Type.String, Type.String) => new Value(YString.Add(left._string, right._string, maxStringLength)),
                _ => throw new InvalidOperationException($"Cannot execute {left._type} + {right._type}")
            };
        }

        public static Value operator +(Value left, Value right)
        {
            return Add(left, right, int.MaxValue);
        }

        public static YString Add(Value left, YString right, int maxStringLength)
        {
            return (left._type) switch
            {
                Type.Number => YString.Add(left._number, right, maxStringLength),
                Type.String => YString.Add(left._string, right, maxStringLength),
                _ => throw new InvalidOperationException($"Cannot execute {left._type} + String")
            };
        }

        public static YString operator +(Value left, YString right)
        {
            return Add(left, right, int.MaxValue);
        }

        public static Value Add(Value l, Number r, int maxStringLength)
        {
            if (l._type == Type.Number)
                return l._number + r;
            else
                return new Value(YString.Add(l._string, r, maxStringLength));
        }

        public static Value operator +(Value l, Number r)
        {
            return Add(l, r, int.MaxValue);
        }

        public static Value Add(Value l, bool r, int maxStringLength)
        {
            return Add(l, (Number)r, maxStringLength);
        }

        public static Value operator +(Value l, bool r)
        {
            return Add(l, r, int.MaxValue);
        }
        #endregion

        #region op *
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool WillMulThrow(Value l, Value r)
        {
            return l._type == Type.String || r._type == Type.String;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool WillMulThrow(Value l, [IgnoreParam] Number _)
        {
            return l._type == Type.String;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool WillMulThrow(Value l, [IgnoreParam] bool _)
        {
            return l._type == Type.String;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Number UnsafeMultiply(Value left, Value right)
        {
            return left._number * right._number;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Number UnsafeMultiply(Value left, Number right)
        {
            return left._number * right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Number UnsafeMultiply(Value left, bool right)
        {
            return left._number * right;
        }

        [ErrorMetadata(nameof(WillMulThrow), nameof(UnsafeMultiply))]
        public static Number operator *([TypeImplication(Type.Number)] Value left, [TypeImplication(Type.Number)] Value right)
        {
            if (left._type == Type.Number && right._type == Type.Number)
                return left._number * right._number;
            else
                throw new ExecutionException("Attempted to multiply a string");
        }

        public static StaticError operator *(Value _, YString __)
        {
            throw new ExecutionException("Attempted to multiply a string");
        }

        [ErrorMetadata(nameof(WillMulThrow), nameof(UnsafeMultiply))]
        public static Number operator *([TypeImplication(Type.Number)] Value left, Number right)
        {
            if (left._type == Type.Number)
                return UnsafeMultiply(left, right);
            else
                throw new ExecutionException("Attempted to multiply a string");
        }

        [ErrorMetadata(nameof(WillMulThrow), nameof(UnsafeMultiply))]
        public static Number operator *([TypeImplication(Type.Number)] Value left, bool right)
        {
            if (left._type == Type.Number)
                return left._number * right;
            else
                throw new ExecutionException("Attempted to multiply a string");
        }
        #endregion

        #region op /
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool WillDivThrow(Value l, Value r)
        {
            if (l._type == Type.Number && r._type == Type.Number)
                return Number.WillDivThrow(l._number, r._number);
            else
                return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool WillDivThrow(Value l, Number r)
        {
            if (l._type == Type.Number)
                return Number.WillDivThrow(l._number, r);
            else
                return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool WillDivThrow(Value l, bool r)
        {
            if (l._type == Type.Number)
                return Number.WillDivThrow(l._number, (Number)r);
            else
                return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Number UnsafeDiv(Value left, Value right)
        {
            return Number.UnsafeDivide(left._number, right._number);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Number UnsafeDiv(Value left, Number right)
        {
            return Number.UnsafeDivide(left._number, right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Number UnsafeDiv(Value left, bool right)
        {
            return Number.UnsafeDivide(left._number, right);
        }

        [ErrorMetadata(nameof(WillDivThrow), nameof(UnsafeDiv))]
        public static Number operator /([TypeImplication(Type.Number)] Value left, [TypeImplication(Type.Number)] Value right)
        {
            if (left._type == Type.Number && right._type == Type.Number)
                return left._number / right._number;
            else
                throw new ExecutionException("Attempted to divide a string");
        }

        public static StaticError operator /(Value _, YString __)
        {
            throw new ExecutionException("Attempted to divide a string");
        }

        [ErrorMetadata(nameof(WillDivThrow), nameof(UnsafeDiv))]
        public static Number operator /([TypeImplication(Type.Number)] Value left, Number right)
        {
            if (left._type == Type.Number)
                return left._number / right;
            else
                throw new ExecutionException("Attempted to divide a string");
        }

        [ErrorMetadata(nameof(WillDivThrow), nameof(UnsafeDiv))]
        public static Number operator /([TypeImplication(Type.Number)] Value left, bool right)
        {
            if (left._type == Type.Number)
                return left._number / right;
            else
                throw new ExecutionException("Attempted to divide a string");
        }
        #endregion

        #region op &
        public static bool operator &(Value left, Value right)
        {
            return left.ToBool() & right.ToBool();
        }
        #endregion

        #region op |
        public static bool operator |(Value left, Value right)
        {
            return left.ToBool() | right.ToBool();
        }
        #endregion

        #region op %
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool WillModThrow(Value l, Value r)
        {
            if (l._type == Type.Number && r._type == Type.Number)
                return Number.WillModThrow(l._number, r._number);
            else
                return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool WillModThrow(Value l, Number r)
        {
            if (l._type == Type.Number)
                return Number.WillModThrow(l._number, r);
            else
                return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool WillModThrow(Value l, bool r)
        {
            if (l._type == Type.Number)
                return Number.WillModThrow(l._number, r);
            else
                return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Number UnsafeMod(Value left, Value right)
        {
            return Number.UnsafeMod(left._number, right._number);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Number UnsafeMod(Value left, Number right)
        {
            return Number.UnsafeMod(left._number, right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Number UnsafeMod(Value left, bool right)
        {
            return Number.UnsafeMod(left._number, right);
        }

        [ErrorMetadata(nameof(WillModThrow), nameof(UnsafeMod))]
        public static Number operator %([TypeImplication(Type.Number)] Value left, [TypeImplication(Type.Number)] Value right)
        {
            if (left._type == Type.Number && right._type == Type.Number)
                return left._number % right._number;
            else
                throw new ExecutionException("Attempted to modulo a string");
        }

        public static StaticError operator %(Value _, YString __)
        {
            return new StaticError("Attempted to modulo a string");
        }

        [ErrorMetadata(nameof(WillModThrow), nameof(UnsafeMod))]
        public static Number operator %([TypeImplication(Type.Number)] Value left, Number right)
        {
            if (left._type == Type.Number)
                return left._number % right;
            else
                throw new ExecutionException("Attempted to modulo a string");
        }

        [ErrorMetadata(nameof(WillModThrow), nameof(UnsafeMod))]
        public static Number operator %([TypeImplication(Type.Number)] Value left, bool right)
        {
            if (left._type == Type.Number)
                return left._number % right;
            else
                throw new ExecutionException("Attempted to modulo a string");
        }
        #endregion

        #region op ++
        public static Value Increment(Value value, int maxStringLength)
        {
            if (value._type == Type.Number)
            {
                var a = value._number;
                a++;
                return new Value(a);
            }
            else
            {
                return new Value(YString.Increment(value._string, maxStringLength));
            }
        }

        public static Value operator ++(Value value)
        {
            return Increment(value, int.MaxValue);
        }
        #endregion

        #region op --
        internal static bool WillDecThrow(Value value)
        {
            return value._type == Type.String
                && value._string.Length == 0;
        }

        [ErrorMetadata(nameof(WillDecThrow))]
        public static Value operator --(Value value)
        {
            if (value._type == Type.Number)
            {
                var a = value._number;
                a--;
                return new Value(a);
            }
            else
            {

                var a = value._string;
                a--;
                return new Value(a);
            }
        }
        #endregion

        #region exponent
        internal static bool WillExponentThrow(Value left, Value right)
        {
            return left.Type != Type.Number || right.Type != Type.Number;
        }

        internal static bool WillExponentThrow(Value left, [IgnoreParam] Number _)
        {
            return left.Type != Type.Number;
        }

        internal static bool WillExponentThrow(Value left, [IgnoreParam] bool _)
        {
            return left.Type != Type.Number;
        }

        internal static Number UnsafeExponent(Value left, Value right)
        {
            return left._number.Exponent(right._number);
        }

        internal static Number UnsafeExponent(Value left, Number right)
        {
            return left._number.Exponent(right);
        }

        internal static Number UnsafeExponent(Value left, bool right)
        {
            return left._number.Exponent(right);
        }

        [ErrorMetadata(nameof(WillExponentThrow), nameof(UnsafeExponent))]
        public static Number Exponent([TypeImplication(Type.Number)] Value left, [TypeImplication(Type.Number)] Value right)
        {
            if (left._type == Type.Number && right._type == Type.Number)
                return left._number.Exponent(right._number);
            else
                throw new ExecutionException("Attempted to exponent a string");
        }

        public static StaticError Exponent(Value _, YString __)
        {
            return new StaticError("Attempted to exponent a string");
        }

        [ErrorMetadata(nameof(WillExponentThrow), nameof(UnsafeExponent))]
        public static Number Exponent([TypeImplication(Type.Number)] Value left, Number right)
        {
            if (left._type == Type.Number)
                return left._number.Exponent(right);
            else
                throw new ExecutionException("Attempted to exponent a string");
        }

        [ErrorMetadata(nameof(WillExponentThrow), nameof(UnsafeExponent))]
        public static Number Exponent([TypeImplication(Type.Number)] Value left, bool right)
        {
            if (left._type == Type.Number)
                return left._number.Exponent(right);
            else
                throw new ExecutionException("Attempted to exponent a string");
        }
        #endregion

        public static bool operator !(Value value)
        {
            return value == Number.Zero;
        }

        #region negate
        internal static bool WillNegateThrow(Value value)
        {
            return value.Type != Type.Number;
        }

        internal static Number UnsafeNegate(Value value)
        {
            return -value._number;
        }

        [ErrorMetadata(nameof(WillNegateThrow), nameof(UnsafeNegate))]
        public static Number operator -([TypeImplication(Type.Number)] Value value)
        {
            if (value._type == Type.String)
                throw new ExecutionException("Attempted to negate a String value");

            return -value._number;
        }
        #endregion

        #region abs
        internal static bool WillAbsThrow(Value value)
        {
            return value.Type != Type.Number;
        }

        internal static Number UnsafeAbs(Value value)
        {
            return value._number.Abs();
        }

        [ErrorMetadata(nameof(WillAbsThrow), nameof(UnsafeAbs))]
        public static Number Abs([TypeImplication(Type.Number)] Value value)
        {
            if (value._type == Type.String)
                throw new ExecutionException("Attempted to Abs a string value");

            return value._number.Abs();
        }
        #endregion

        #region sqrt
        internal static bool WillSqrtThrow(Value value)
        {
            return value.Type != Type.Number;
        }

        internal static Number UnsafeSqrt(Value value)
        {
            return value._number.Sqrt();
        }

        [ErrorMetadata(nameof(WillSqrtThrow), nameof(UnsafeSqrt))]
        public static Number Sqrt([TypeImplication(Type.Number)] Value value)
        {
            if (value._type == Type.String)
                throw new ExecutionException("Attempted to Sqrt a string value");

            return value._number.Sqrt();
        }
        #endregion

        #region sin
        internal static bool WillSinThrow(Value value)
        {
            return value.Type != Type.Number;
        }

        internal static Number UnsafeSin(Value value)
        {
            return value._number.Sin();
        }

        [ErrorMetadata(nameof(WillSinThrow), nameof(UnsafeSin))]
        public static Number Sin([TypeImplication(Type.Number)] Value value)
        {
            if (value._type == Type.String)
                throw new ExecutionException($"Attempted to `Sin` a string value");

            return value._number.Sin();
        }
        #endregion

        #region cos
        internal static bool WillCosThrow(Value value)
        {
            return value.Type != Type.Number;
        }

        internal static Number UnsafeCos(Value value)
        {
            return value._number.Cos();
        }

        [ErrorMetadata(nameof(WillCosThrow), nameof(UnsafeCos))]
        public static Number Cos([TypeImplication(Type.Number)] Value value)
        {
            if (value._type == Type.String)
                throw new ExecutionException($"Attempted to `Cos` a string value");

            return value._number.Cos();
        }
        #endregion

        #region tan
        internal static bool WillTanThrow(Value value)
        {
            return value.Type != Type.Number;
        }

        internal static Number UnsafeTan(Value value)
        {
            return value._number.Tan();
        }

        [ErrorMetadata(nameof(WillTanThrow), nameof(UnsafeTan))]
        public static Number Tan([TypeImplication(Type.Number)] Value value)
        {
            if (value._type == Type.String)
                throw new ExecutionException($"Attempted to `Tan` a string value");
            return value._number.Tan();
        }
        #endregion

        #region atan
        internal static bool WillAtanThrow(Value value)
        {
            return value.Type != Type.Number;
        }

        internal static Number UnsafeAtan(Value value)
        {
            return value._number.ArcTan();
        }

        [ErrorMetadata(nameof(WillAtanThrow), nameof(UnsafeAtan))]
        public static Number ArcTan([TypeImplication(Type.Number)] Value value)
        {
            if (value._type == Type.String)
                throw new ExecutionException($"Attempted to `ATan` a string value");

            return value._number.ArcTan();
        }
        #endregion

        #region asin
        internal static bool WillArcSinThrow(Value value)
        {
            return value.Type != Type.Number;
        }

        internal static Number UnsafeArcSin(Value value)
        {
            return value._number.ArcSin();
        }

        [ErrorMetadata(nameof(WillArcSinThrow), nameof(UnsafeArcSin))]
        public static Number ArcSin([TypeImplication(Type.Number)] Value value)
        {
            if (value._type == Type.String)
                throw new ExecutionException($"Attempted to `ASin` a string value");

            return value._number.ArcSin();
        }
        #endregion

        #region acos
        internal static bool WillArcCosThrow(Value value)
        {
            return value.Type != Type.Number;
        }

        internal static Number UnsafeArcCos(Value value)
        {
            return value._number.ArcCos();
        }

        [ErrorMetadata(nameof(WillArcCosThrow), nameof(UnsafeArcCos))]
        public static Number ArcCos([TypeImplication(Type.Number)] Value value)
        {
            if (value._type == Type.String)
                throw new ExecutionException($"Attempted to `ACos` a string value");

            return value._number.ArcCos();
        }
        #endregion

        public BaseExpression ToConstant()
        {
            if (_type == Type.Number)
                return new ConstantNumber(_number);
            else
                return new ConstantString(String);
        }

        #region factorial
        internal static bool WillFactorialThrow(Value value)
        {
            return value.Type != Type.Number;
        }

        internal static Number UnsafeFactorial(Value value)
        {
            return value._number.Factorial();
        }

        public Value Factorial()
        {
            if (_type == Type.String)
                throw new ExecutionException("Attempted to apply factorial to a string");
            
            return _number.Factorial();
        }

        [ErrorMetadata(nameof(WillFactorialThrow), nameof(UnsafeFactorial))]
        public static Value Factorial([TypeImplication(Type.Number)] Value value)
        {
            if (value.Type == Type.String)
                throw new ExecutionException("Attempted to apply factorial to a string");
            
            return value._number.Factorial();
        }
        #endregion
    }
}

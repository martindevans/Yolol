using System;
using System.Runtime.CompilerServices;
using Yolol.Execution.Attributes;
using Yolol.Grammar.AST.Expressions;

namespace Yolol.Execution
{
    public readonly struct Value
        : IEquatable<Value>
    {
        private readonly Type _type;
        public Type Type => _type;

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

        internal Number UnsafeNumber => _number;
        internal YString UnsafeString => _string;

        public Value(string str)
        {
            _string = new YString(str);
            _number = Number.Zero;
            _type = Type.String;
        }

        public Value(YString str)
        {
            _string = str;
            _number = Number.Zero;
            _type = Type.String;
        }

        public Value(Number num)
        {
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

        public override bool Equals(object obj)
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

        public static bool operator <(Value left, YString right)
        {
            if (left._type == Type.Number)
                return left._number < right;
            else
                return left._string < right;
        }

        public static bool operator <(Value left, Number right)
        {
            if (left._type == Type.Number)
                return left._number < right;
            else
                return left._string < right;
        }

        public static bool operator <(Value left, bool right)
        {
            if (left._type == Type.Number)
                return left._number < right;
            else
                return left._string < right;
        }
        #endregion

        #region op <=
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

        public static bool operator <=(Value left, YString right)
        {
            if (left._type == Type.Number)
                return left._number <= right;
            else
                return left._string <= right;
        }

        public static bool operator <=(Value left, Number right)
        {
            if (left._type == Type.Number)
                return left._number <= right;
            else
                return left._string <= right;
        }

        public static bool operator <=(Value left, bool right)
        {
            if (left._type == Type.Number)
                return left._number <= right;
            else
                return left._string <= right;
        }
        #endregion

        #region op >
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

        public static bool operator >(Value left, YString right)
        {
            if (left._type == Type.Number)
                return left._number > right;
            else
                return left._string > right;
        }

        public static bool operator >(Value left, Number right)
        {
            if (left._type == Type.Number)
                return left._number > right;
            else
                return left._string > right;
        }

        public static bool operator >(Value left, bool right)
        {
            if (left._type == Type.Number)
                return left._number > right;
            else
                return left._string > right;
        }
        #endregion

        #region op >=
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

        public static bool operator >=(Value left, YString right)
        {
            if (left._type == Type.Number)
                return left._number >= right;
            else
                return left._string >= right;
        }

        public static bool operator >=(Value left, Number right)
        {
            if (left._type == Type.Number)
                return left._number >= right;
            else
                return left._string >= right;
        }

        public static bool operator >=(Value left, bool right)
        {
            if (left._type == Type.Number)
                return left._number >= right;
            else
                return left._string >= right;
        }
        #endregion

        #region op ==
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

        public static bool operator ==(Value left, YString right)
        {
            if (left._type != Type.String)
                return false;
            else
                return left._string == right;
        }

        public static bool operator ==(Value left, Number right)
        {
            if (left._type != Type.Number)
                return false;
            else
                return left._number == right;
        }

        public static bool operator ==(Value left, bool right)
        {
            if (left._type != Type.Number)
                return false;
            else
                return left._number == right;
        }
        #endregion

        #region op !=
        public static bool operator !=(Value left, Value right)
        {
            return !(left == right);
        }

        public static bool operator !=(Value left, YString right)
        {
            if (left._type != Type.String)
                return true;
            else
                return left._string != right;
        }

        public static bool operator !=(Value left, Number right)
        {
            if (left._type != Type.Number)
                return true;
            else
                return left._number != right;
        }

        public static bool operator !=(Value left, bool right)
        {
            if (left._type != Type.Number)
                return true;
            else
                return left._number != right;
        }
        #endregion

        #region op -
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

        public static YString operator -(Value left, YString right)
        {
            return left.ToYString() - right;
        }

        public static Value operator -(Value l, Number r)
        {
            if (l._type == Type.Number)
                return l._number - r;
            else
                return new Value(l._string - r);
        }

        public static Value operator -(Value l, bool r)
        {
            return l - (Number)r;
        }
        #endregion

        #region op +
        public static Value operator +(Value left, Value right)
        {
            return (left._type, right._type) switch {
                (Type.Number, Type.Number) => left._number + right._number,
                (Type.Number, Type.String) => new Value(left._number + right._string),
                (Type.String, Type.Number) => new Value(left._string + right._number),
                (Type.String, Type.String) => new Value(left._string + right._string),
                _ => throw new InvalidOperationException($"Cannot execute {left._type} + {right._type}")
            };
        }

        public static YString operator +(Value left, YString right)
        {
            return left.ToYString() + right;
        }

        public static Value operator +(Value l, Number r)
        {
            if (l._type == Type.Number)
                return l._number + r;
            else
                return new Value(l._string + r);
        }

        public static Value operator +(Value l, bool r)
        {
            return l + (Number)r;
        }
        #endregion

        #region op *
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool WillMulThrow(Value l, Value r)
        {
            return l._type == Type.String || r._type == Type.String;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool WillMulThrow(Value l, Number _)
        {
            return l._type == Type.String;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool WillMulThrow(Value l, bool _)
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
        public static Number operator *(Value left, Value right)
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
        public static Number operator *(Value left, Number right)
        {
            if (left._type == Type.Number)
                return UnsafeMultiply(left, right);
            else
                throw new ExecutionException("Attempted to multiply a string");
        }

        [ErrorMetadata(nameof(WillMulThrow), nameof(UnsafeMultiply))]
        public static Number operator *(Value left, bool right)
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
        public static Number operator /(Value left, Value right)
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
        public static Number operator /(Value left, Number right)
        {
            if (left._type == Type.Number)
                return left._number / right;
            else
                throw new ExecutionException("Attempted to divide a string");
        }

        [ErrorMetadata(nameof(WillDivThrow), nameof(UnsafeDiv))]
        public static Number operator /(Value left, bool right)
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
        public static Number operator %(Value left, Value right)
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
        public static Number operator %(Value left, Number right)
        {
            if (left._type == Type.Number)
                return left._number % right;
            else
                throw new ExecutionException("Attempted to modulo a string");
        }

        [ErrorMetadata(nameof(WillModThrow), nameof(UnsafeMod))]
        public static Number operator %(Value left, bool right)
        {
            if (left._type == Type.Number)
                return left._number % right;
            else
                throw new ExecutionException("Attempted to modulo a string");
        }
        #endregion

        #region op ++
        public static Value operator ++(Value value)
        {
            if (value._type == Type.Number)
            {
                var a = value._number;
                a++;
                return new Value(a);
            }
            else
            {
                var a = value._string;
                a++;
                return new Value(a);
            }
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

        internal static bool WillExponentThrow(Value left, Number _)
        {
            return left.Type != Type.Number;
        }

        internal static bool WillExponentThrow(Value left, bool _)
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
        public static Number Exponent(Value left, Value right)
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
        public static Number Exponent(Value left, Number right)
        {
            if (left._type == Type.Number)
                return left._number.Exponent(right);
            else
                throw new ExecutionException("Attempted to exponent a string");
        }

        [ErrorMetadata(nameof(WillExponentThrow), nameof(UnsafeExponent))]
        public static Number Exponent(Value left, bool right)
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
        public static Number operator -(Value value)
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
        public static Number Abs(Value value)
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
        public static Number Sqrt(Value value)
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
        public static Number Sin(Value value)
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
        public static Number Cos(Value value)
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
        public static Number Tan(Value value)
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
        public static Number ArcTan(Value value)
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
        public static Number ArcSin(Value value)
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
        public static Number ArcCos(Value value)
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

        [ErrorMetadata(nameof(WillFactorialThrow), nameof(UnsafeFactorial))]
        public Value Factorial()
        {
            if (_type == Type.String)
                throw new ExecutionException("Attempted to apply factorial to a string");
            
            return _number.Factorial();
        }
        #endregion
    }
}

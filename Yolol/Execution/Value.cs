using System;
using System.Runtime.InteropServices;
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

        public static implicit operator Value(decimal d)
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
                return true;
            else
                return _number != 0;
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
            if (left._type == Type.Number)
                return left._number < right;
            else
                return left._string < right;
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
            if (left._type == Type.Number)
                return left._number <= right;
            else
                return left._string <= right;
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
            if (left._type == Type.Number)
                return left._number > right;
            else
                return left._string > right;
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
            if (left._type == Type.Number)
                return left._number >= right;
            else
                return left._string >= right;
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
            return left.Equals(right);
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
            return !left.Equals(right);
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
            if (left._type == Type.Number && right._type == Type.Number)
                return left._number - right._number;
            else
                return new Value(left.ToYString() - right.ToYString());
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
            if (left._type == Type.Number && right._type == Type.Number)
                return left._number + right._number;
            else
                return new Value(left.ToYString() + right.ToYString());
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
        internal static bool WillMulThrow(Value l, Value r)
        {
            return l._type == Type.String || r._type == Type.String;
        }

        internal static bool WillMulThrow(Value l, Number _)
        {
            return l._type == Type.String;
        }

        internal static bool WillMulThrow(Value l, bool _)
        {
            return l._type == Type.String;
        }

        [ErrorMetadata(nameof(WillMulThrow))]
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

        [ErrorMetadata(nameof(WillMulThrow))]
        public static Number operator *(Value left, Number right)
        {
            if (left._type == Type.Number)
                return left._number * right;
            else
                throw new ExecutionException("Attempted to multiply a string");
        }

        [ErrorMetadata(nameof(WillMulThrow))]
        public static Number operator *(Value left, bool right)
        {
            if (left._type == Type.Number)
                return left._number * right;
            else
                throw new ExecutionException("Attempted to multiply a string");
        }
        #endregion

        #region op /
        internal static bool WillDivThrow(Value l, Value r)
        {
            if (l._type == Type.Number && r._type == Type.Number)
                return Number.WillDivThrow(l.Number, r.Number);
            else
                return true;
        }

        internal static bool WillDivThrow(Value l, Number r)
        {
            if (l._type == Type.Number)
                return Number.WillDivThrow(l.Number, r);
            else
                return true;
        }

        internal static bool WillDivThrow(Value l, bool r)
        {
            if (l._type == Type.Number)
                return Number.WillDivThrow(l.Number, (Number)r);
            else
                return true;
        }

        [ErrorMetadata(nameof(WillDivThrow))]
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

        [ErrorMetadata(nameof(WillDivThrow))]
        public static Number operator /(Value left, Number right)
        {
            if (left._type == Type.Number)
                return left._number / right;
            else
                throw new ExecutionException("Attempted to divide a string");
        }

        [ErrorMetadata(nameof(WillDivThrow))]
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
        internal static bool WillModThrow(Value l, Value r)
        {
            if (l._type == Type.Number && r._type == Type.Number)
                return Number.WillModThrow(l.Number, r.Number);
            else
                return true;
        }

        internal static bool WillModThrow(Value l, Number r)
        {
            if (l._type == Type.Number)
                return Number.WillModThrow(l.Number, r);
            else
                return true;
        }

        internal static bool WillModThrow(Value l, bool r)
        {
            if (l._type == Type.Number)
                return Number.WillModThrow(l.Number, r);
            else
                return true;
        }

        [ErrorMetadata(nameof(WillModThrow))]
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

        [ErrorMetadata(nameof(WillModThrow))]
        public static Number operator %(Value left, Number right)
        {
            if (left._type == Type.Number)
                return left._number % right;
            else
                throw new ExecutionException("Attempted to modulo a string");
        }

        [ErrorMetadata(nameof(WillModThrow))]
        public static Number operator %(Value left, bool right)
        {
            if (left._type == Type.Number)
                return left._number % (Number)right;
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

        public static Number Exponent(Value left, Number right)
        {
            if (left._type == Type.Number)
                return left._number.Exponent(right);
            else
                throw new ExecutionException("Attempted to exponent a string");
        }

        public static Number Exponent(Value left, bool right)
        {
            if (left._type == Type.Number)
                return left._number.Exponent(right);
            else
                throw new ExecutionException("Attempted to exponent a string");
        }


        public static bool operator !(Value value)
        {
            return value == Number.Zero;
        }

        public static Number operator -(Value value)
        {
            if (value._type == Type.String)
                throw new ExecutionException("Attempted to negate a String value");

            return -value._number;
        }

        public static Number Abs(Value value)
        {
            if (value._type == Type.String)
                throw new ExecutionException("Attempted to Abs a string value");

            return value._number.Abs();
        }

        public static Number Sqrt(Value value)
        {
            if (value._type == Type.String)
                throw new ExecutionException("Attempted to Sqrt a string value");

            return value._number.Sqrt();
        }

        public static Number Sin(Value value)
        {
            if (value._type == Type.String)
                throw new ExecutionException($"Attempted to `Sin` a string value");

            return value._number.Sin();
        }

        public static Number Cos(Value value)
        {
            if (value._type == Type.String)
                throw new ExecutionException($"Attempted to `Cos` a string value");

            return value._number.Cos();
        }

        public static Number Tan(Value value)
        {
            if (value._type == Type.String)
                throw new ExecutionException($"Attempted to `Tan` a string value");
            return value._number.Tan();
        }

        public static Number ArcTan(Value value)
        {
            if (value._type == Type.String)
                throw new ExecutionException($"Attempted to `ATan` a string value");

            return value._number.ArcTan();
        }

        public static Number ArcSin(Value value)
        {
            if (value._type == Type.String)
                throw new ExecutionException($"Attempted to `ASin` a string value");

            return value._number.ArcSin();
        }

        public static Number ArcCos(Value value)
        {
            if (value._type == Type.String)
                throw new ExecutionException($"Attempted to `ACos` a string value");

            return value._number.ArcCos();
        }

        public BaseExpression ToConstant()
        {
            if (_type == Type.Number)
                return new ConstantNumber(_number);
            else
                return new ConstantString(String);
        }

        public Value Factorial()
        {
            if (_type == Type.String)
                throw new ExecutionException("Attempted to apply factorial to a string");
            
            if (_type != Type.Number)
                throw new ExecutionException($"Attempted to apply factorial to a `{_type}` object");

            return Number.Factorial();
        }
    }
}

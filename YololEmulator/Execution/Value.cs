using System;
using System.Globalization;

namespace YololEmulator.Execution
{
    public struct Value
    {
        public Type Type { get; private set; }

        private decimal _valueNumber;
        public decimal ValueNumber
        {
            get
            {
                if (Type != Type.Number)
                    throw new InvalidCastException($"Attempted to access value of type {Type} as a Number");
                return _valueNumber;
            }
            set
            {
                Type = Type.Number;
                _valueNumber = value;
            }
        }

        private string _valueString;
        public string ValueString
        {
            get
            {
                if (Type != Type.String)
                    throw new InvalidCastException($"Attempted to access value of type {Type} as a String");
                return _valueString;
            }
            set
            {
                Type = Type.String;
                _valueString = value;
            }
        }

        public Value(string str)
        {
            _valueString = str;
            _valueNumber = 0;
            Type = Type.String;
        }

        public Value(decimal dec)
        {
            _valueString = "";
            _valueNumber = dec;
            Type = Type.Number;
        }

        public override string ToString()
        {
            if (Type == Type.None)
                return "<unassigned>";

            if (Type == Type.Number)
                return ValueNumber.ToString(CultureInfo.InvariantCulture);

            return $"\"{ValueString}\"";
        }
    }
}

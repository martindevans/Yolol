using System;
using System.Globalization;

namespace YololEmulator.Execution
{
    public struct Value
    {
        public Type Type { get; private set; }

        private decimal _number;
        public decimal Number
        {
            get
            {
                if (Type != Type.Number)
                    throw new InvalidCastException($"Attempted to access value of type {Type} as a Number");
                return _number;
            }
            set
            {
                Type = Type.Number;
                _number = value;
            }
        }

        private string _string;
        public string String
        {
            get
            {
                if (Type != Type.String)
                    throw new InvalidCastException($"Attempted to access value of type {Type} as a String");
                return _string;
            }
            set
            {
                Type = Type.String;
                _string = value;
            }
        }

        public Value(string str)
        {
            _string = str;
            _number = 0;
            Type = Type.String;
        }

        public Value(decimal dec)
        {
            _string = "";
            _number = dec;
            Type = Type.Number;
        }

        public override string ToString()
        {
            if (Type == Type.None)
                return "<unassigned>";

            if (Type == Type.Number)
                return Number.ToString(CultureInfo.InvariantCulture);

            return $"\"{String}\"";
        }
    }
}

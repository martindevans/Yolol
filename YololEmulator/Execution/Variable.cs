using System;

namespace YololEmulator.Execution
{
    public class Variable
    {

        public bool IsExternal { get; }

        public string Name { get; }

        private Value _value;
        public Value Value
        {
            get
            {
                if (IsExternal)
                    throw new NotImplementedException("get external var");
                return _value;
            }
            set => _value = value;
        }

        public Variable(string name, bool external)
        {
            Name = name;
            IsExternal = external;
            Value = new Value();
        }

        public override string ToString()
        {
            return _value.ToString();
        }
    }
}

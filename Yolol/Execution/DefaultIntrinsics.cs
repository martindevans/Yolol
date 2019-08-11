using System;
using System.Collections;
using System.Collections.Generic;

namespace Yolol.Execution
{
    public class DefaultIntrinsics
        : IReadOnlyDictionary<string, Func<Value, Value>>
    {
        private readonly IReadOnlyDictionary<string, Func<Value, Value>> _intrinsics;

        public DefaultIntrinsics()
        {
            _intrinsics = new Dictionary<string, Func<Value, Value>> {
                { "abs", a => Math.Abs(a.Number.Value) },
                { "sqrt", a => (decimal)Math.Sqrt((double)a.Number.Value) },
                { "sin", a => (decimal)Math.Sin(Radians(a.Number.Value)) },
                { "cos", a => (decimal)Math.Cos(Radians(a.Number.Value)) },
                { "tan", a => (decimal)Math.Tan(Radians(a.Number.Value)) },
                { "asin", a => Degrees(Math.Asin((double)a.Number.Value)) },
                { "acos", a => Degrees(Math.Acos((double)a.Number.Value)) },
                { "atan", a => Degrees(Math.Atan((double)a.Number.Value)) },
            };
        }

        private static decimal Degrees(double radians)
        {
            return (decimal)(radians * (180.0 / Math.PI));
        }

        private static double Radians(decimal degrees)
        {
            return Math.PI * (double)degrees / 180.0;
        }

        public IEnumerator<KeyValuePair<string, Func<Value, Value>>> GetEnumerator()
        {
            return _intrinsics.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_intrinsics).GetEnumerator();
        }

        public int Count => _intrinsics.Count;

        public bool ContainsKey(string key)
        {
            return _intrinsics.ContainsKey(key);
        }

        public bool TryGetValue(string key, out Func<Value, Value> value)
        {
            return _intrinsics.TryGetValue(key, out value);
        }

        public Func<Value, Value> this[string key] => _intrinsics[key];

        public IEnumerable<string> Keys => _intrinsics.Keys;

        public IEnumerable<Func<Value, Value>> Values => _intrinsics.Values;
    }
}

using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;
using Type = Yolol.Execution.Type;

namespace YololEmulator.Tests
{
    [TestClass]
    public class ValueTests
    {
        private static void AssertNumber(Value v, decimal d)
        {
            Assert.AreEqual(Type.Number, v.Type);
            Assert.AreEqual(d, v.Number);
            Assert.AreEqual(d.ToString(CultureInfo.InvariantCulture), v.ToString());

            Assert.ThrowsException<InvalidCastException>(() => {
                var _ = v.String;
            });
        }

        private static void AssertString(Value v, string s)
        {
            Assert.AreEqual(Type.String, v.Type);
            Assert.AreEqual(s, v.String);
            Assert.AreEqual(s, v.ToString());

            Assert.ThrowsException<InvalidCastException>(() => {
                var _ = v.Number;
            });
        }

        [TestMethod]
        public void String()
        {
            var s = new Value("a");

            AssertString(s, "a");
        }

        [TestMethod]
        public void Number()
        {
            var n = new Value(1);

            AssertNumber(n, 1);
        }

        [TestMethod]
        public void StringStringEquality()
        {
            var a1 = new Value("a");
            var a2 = new Value("a");
            var b = new Value("b");

            Assert.AreEqual(a1.ToObject(), a2.ToObject());
            Assert.AreEqual(a1, a2);

            Assert.AreEqual(a1.GetHashCode(), a2.GetHashCode());
        }

        [TestMethod]
        public void NumNumEquality()
        {
            var a1 = new Value(1);
            var a2 = new Value(1);
            var b = new Value(2);

            Assert.AreEqual(a1.ToObject(), a2.ToObject());
            Assert.AreEqual(a1, a2);

            Assert.AreEqual(a1.GetHashCode(), a2.GetHashCode());
        }

        [TestMethod]
        public void StringNumEquality()
        {
            var a1 = new Value(1);
            var a2 = new Value(1);
            var b = new Value("2");

            Assert.AreEqual(a1.ToObject(), a2.ToObject());
            Assert.AreEqual(a1, a2);

            Assert.AreEqual(a1.GetHashCode(), a2.GetHashCode());
        }
    }
}

using System;
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
            Assert.AreEqual(d.ToString(), v.ToString());

            Assert.ThrowsException<InvalidCastException>(() => {
                var s = v.String;
            });
        }

        private static void AssertString(Value v, string s)
        {
            Assert.AreEqual(Type.String, v.Type);
            Assert.AreEqual(s, v.String);
            Assert.AreEqual($"\"{s}\"", v.ToString());

            Assert.ThrowsException<InvalidCastException>(() => {
                var n = v.Number;
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
    }
}

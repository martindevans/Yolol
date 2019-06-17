using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YololEmulator.Execution;

namespace YololEmulator.Tests
{
    [TestClass]
    public class ValueTests
    {
        private static void AssertNumber(Value v, decimal d)
        {
            Assert.AreEqual(Execution.Type.Number, v.Type);
            Assert.AreEqual(d, v.Number);
            Assert.AreEqual(d.ToString(), v.ToString());

            Assert.ThrowsException<InvalidCastException>(() => {
                var s = v.String;
            });
        }

        private static void AssertString(Value v, string s)
        {
            Assert.AreEqual(Execution.Type.String, v.Type);
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

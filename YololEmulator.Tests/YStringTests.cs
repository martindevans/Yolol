using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests
{
    [TestClass]
    public class YStringTests
    {
        [TestMethod]
        public void YStringToString()
        {
            var s = new YString("abc");
            Assert.AreEqual("abc", s.ToString());
        }

        [TestMethod]
        public void ConcatSimple()
        {
            var a = new YString("abc");
            var b = new YString("def");
            var c = a + b;

            Assert.AreEqual("abcdef", c.ToString());
        }

        [TestMethod]
        public void ConcatCharComplex()
        {
            var a = new YString("abc");
            a--;

            var c = a + true;

            Assert.AreEqual("ab1", c.ToString());
        }

        [TestMethod]
        public void CompareLessThan()
        {
            var a = new YString("abc");
            var b = new YString("def");

            Assert.IsTrue(a < b);
        }

        [TestMethod]
        public void CompareNotLessThan()
        {
            var a = new YString("abc");
            var b = new YString("def");

            Assert.IsFalse(b < a);
        }

        [TestMethod]
        public void CompareGreaterThan()
        {
            var a = new YString("abc");
            var b = new YString("def");

            Assert.IsTrue(b > a);
        }

        [TestMethod]
        public void CompareNotGreaterThan()
        {
            var a = new YString("abc");
            var b = new YString("def");

            Assert.IsFalse(a > b);
        }

        [TestMethod]
        public void SubtractNotFound()
        {
            var a = new YString("abc");
            var b = new YString("def");
            var c = a - b;

            Assert.AreEqual("abc", c.ToString());
        }

        [TestMethod]
        public void SubtractFromEnd()
        {
            var a = new YString("abc");
            var b = new YString("c");
            var c = a - b;

            Assert.AreEqual("ab", c.ToString());
        }

        [TestMethod]
        public void SubtractFromMiddle()
        {
            var a = new YString("abc");
            var b = new YString("b");
            var c = a - b;

            Assert.AreEqual("ac", c.ToString());
        }

        [TestMethod]
        public void SubtractFromStart()
        {
            var a = new YString("abc");
            var b = new YString("a");
            var c = a - b;

            Assert.AreEqual("bc", c.ToString());
        }

        [TestMethod]
        public void SubtractLongNotFound()
        {
            var a = new YString("awr82unm350bc");
            var b = new YString("de3v489u6n28f");
            var c = a - b;

            Assert.AreEqual("awr82unm350bc", c.ToString());
        }

        [TestMethod]
        public void SubtractLongFromEnd()
        {
            var a = new YString("abcdefghijklmnopqrstuv");
            var b = new YString("tuv");
            var c = a - b;

            Assert.AreEqual("abcdefghijklmnopqrs", c.ToString());
        }

        [TestMethod]
        public void SubtractLongFromMiddle()
        {
            var a = new YString("abcdefghijklmnopqrstuv");
            var b = new YString("hijk");
            var c = a - b;

            Assert.AreEqual("abcdefglmnopqrstuv", c.ToString());
        }

        [TestMethod]
        public void SubtractLongFromMiddleWithpartialMatch()
        {
            var a = new YString("abcdefghijklmnopqrstuvhijksdfjhklmnm");
            var b = new YString("hijklm");
            var c = a - b;

            Assert.AreEqual("abcdefgnopqrstuvhijksdfjhklmnm", c.ToString());
        }

        [TestMethod]
        public void SubtractLongFromStart()
        {
            var a = new YString("abcdefghijklmnopqrstuv");
            var b = new YString("abcdef");
            var c = a - b;

            Assert.AreEqual("ghijklmnopqrstuv", c.ToString());
        }

        [TestMethod]
        public void SubtractLastItem()
        {
            var a = new YString("abcdefghijklmnabcdefghijklmn");
            var b = new YString("abcdef");
            var c = a - b;

            Assert.AreEqual("abcdefghijklmnghijklmn", c.ToString());
        }

        [TestMethod]
        public void SubtractEmptyString()
        {
            var a = new YString("abcdefghijklmnabcdefghijklmn");
            var b = new YString("");
            var c = a - b;

            Assert.AreEqual("abcdefghijklmnabcdefghijklmn", c.ToString());
        }

        [TestMethod]
        public void SubtractFromEmptyString()
        {
            var a = new YString("");
            var b = new YString("54745");
            var c = a - b;

            Assert.AreEqual("", c.ToString());
        }

        [TestMethod]
        public void SubtractEmptyFromEmptyString()
        {
            var a = new YString("");
            var b = new YString("");
            var c = a - b;

            Assert.AreEqual("", c.ToString());
        }

        [TestMethod]
        public void SubtractNumber()
        {
            var a = new YString("58dsfg35y");
            var b = (Number)35;
            var c = a - b;

            Assert.AreEqual("58dsfgy", c.ToString());
        }

        [TestMethod]
        public void SubtractBool()
        {
            var a = new YString("58dsfg135y");
            var c = a - true;

            Assert.AreEqual("58dsfg35y", c.ToString());
        }
    }
}

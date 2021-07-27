using System;
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
        public void AddToEmptyString()
        {
            var a = new YString("");
            var b = new YString("abcd");
            var c = a + b;

            Assert.AreEqual("abcd", c.ToString());
        }

        [TestMethod]
        public void AddNumberToEmptyString()
        {
            var a = new YString("");
            var b = (Number)7;
            var c = a + b;

            Assert.AreEqual("7", c.ToString());
        }

        [TestMethod]
        public void AddEmptyStringToNumber()
        {
            var a = (Number)7;
            var b = new YString("");
            var c = a + b;

            Assert.AreEqual("7", c.ToString());
        }

        [TestMethod]
        public void AddEmptyString()
        {
            var a = new YString("abcd");
            var b = new YString("");
            var c = a + b;

            Assert.AreEqual("abcd", c.ToString());
        }

        [TestMethod]
        public void AddStrings()
        {
            var a = new YString("abc");
            var b = new YString("d");
            var c = a + b;

            Assert.AreEqual("abcd", c.ToString());
        }

        [TestMethod]
        public void AddNumber()
        {
            var a = new YString("abc");
            var b = (Number)7;
            var c = a + b;

            Assert.AreEqual("abc7", c.ToString());
        }

        [TestMethod]
        public void AddNumberToShortenedString()
        {
            var a = new YString("abc");
            a--;
            var b = (Number)7;
            var c = a + b;

            Assert.AreEqual("ab7", c.ToString());
        }

        [TestMethod]
        public void AddStringToShortenedStrings()
        {
            var a = new YString("abc");
            a--;
            var b = new YString("d");
            var c = a + b;

            Assert.AreEqual("abd", c.ToString());
        }

        [TestMethod]
        public void AddStringToShortenedStartStrings()
        {
            var a = new YString("abc");
            a -= "a";
            var c = a + true;

            Assert.AreEqual("bc1", c.ToString());
        }

        [TestMethod]
        public void AddStringToShortenedEndStrings()
        {
            var a = new YString("abc");
            a -= "c";
            var c = a + true;

            Assert.AreEqual("ab1", c.ToString());
        }

        [TestMethod]
        public void AddStringToShortenedBothStrings()
        {
            var a = new YString("abc");
            a -= "c";
            a -= "a";
            var c = a + true;

            Assert.AreEqual("b1", c.ToString());
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
            var a = new YString("1abc");
            var c = a - true;

            Assert.AreEqual("abc", c.ToString());
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
        public void SubtractLongFromMiddleWithPartialMatch()
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

        [TestMethod]
        public void SubtractNoMatch()
        {
            var a = new YString("58dsfg135y");
            var c = a - "nothing";

            Assert.AreEqual("58dsfg135y", c.ToString());
        }

        [TestMethod]
        public void SubtractNoMatchNumber()
        {
            var a = new YString("58dsfg135y");
            var c = a - (Number)7;

            Assert.AreEqual("58dsfg135y", c.ToString());
        }

        [TestMethod]
        public void SubtractNumberFromEnd()
        {
            var a = new YString("58dsfg135y7");
            var c = a - (Number)7;

            Assert.AreEqual("58dsfg135y", c.ToString());
        }

        [TestMethod]
        public void SubtractNumberFromEmpty()
        {
            var a = new YString("");
            var c = a - (Number)7;

            Assert.AreEqual("", c.ToString());
        }

        [TestMethod]
        public void StringOrderSameLength()
        {
            var a = new YString("abc");
            var b = new YString("bcd");

            Assert.IsTrue(a < b);
            Assert.IsTrue(a <= b);
            Assert.IsTrue(a != b);
            Assert.IsFalse(a == b);
            Assert.IsFalse(b == a);
            Assert.IsTrue(b != a);
            Assert.IsTrue(b >= a);
            Assert.IsTrue(b > a);
        }

        [TestMethod]
        public void StringOrderIdentical()
        {
            var a = new YString("abc");
            var b = new YString("abc");

            Assert.IsFalse(a < b);
            Assert.IsTrue(a <= b);
            Assert.IsFalse(a != b);
            Assert.IsTrue(a == b);
            Assert.IsTrue(b == a);
            Assert.IsFalse(b != a);
            Assert.IsTrue(b >= a);
            Assert.IsFalse(b > a);
        }

        [TestMethod]
        public void StringOrderLonger()
        {
            var a = new YString("abc");
            var b = new YString("abcd");

            Assert.IsTrue(a < b);
            Assert.IsTrue(a <= b);
            Assert.IsTrue(a != b);
            Assert.IsFalse(a == b);
            Assert.IsFalse(b == a);
            Assert.IsTrue(b != a);
            Assert.IsTrue(b >= a);
            Assert.IsTrue(b > a);
        }

        [TestMethod]
        public void StringOrderNumbers()
        {
            var a = new YString("111");
            var b = new YString("12");

            Assert.IsTrue(a < b);
            Assert.IsTrue(a <= b);
            Assert.IsTrue(a != b);
            Assert.IsFalse(a == b);
            Assert.IsFalse(b == a);
            Assert.IsTrue(b != a);
            Assert.IsTrue(b >= a);
            Assert.IsTrue(b > a);
        }

        [TestMethod]
        public void StringOrderFuzz()
        {
            var rng = new Random(3463245);

            string RandomString()
            {
                var bytes = new byte[rng.Next(1, 25)];
                rng.NextBytes(bytes);
                var chars = new char[bytes.Length];
                bytes.CopyTo(chars, 0);

                return new string(chars);
            }

            for (var i = 0; i < 1000; i++)
            {
                var a = RandomString();
                var b = RandomString();

                var ay = new YString(a);
                var by = new YString(b);

                var order = StringComparer.Ordinal.Compare(a, b);

                Assert.AreEqual(order < 0, ay < by);
                Assert.AreEqual(order <= 0, ay <= by);
                Assert.AreEqual(order != 0, ay != by);
                Assert.AreEqual(order == 0, ay == by);
                Assert.AreEqual(order == 0, by == ay);
                Assert.AreEqual(order != 0, by != ay);
                Assert.AreEqual(order >= 0, ay >= by);
                Assert.AreEqual(order > 0, ay > by);
            }
        }

        [TestMethod]
        public void StringEquals()
        {
            var a = new YString("1234");
            Assert.IsTrue(a.Equals("1234"));
        }

        [TestMethod]
        public void EmptyStringEquals()
        {
            var a = new YString("");
            Assert.IsTrue(a.Equals(""));
        }

        [TestMethod]
        public void PopLastCharacter()
        {
            var str = new YString("Hello");
            Assert.AreEqual("o", str.LastCharacter().ToString());
        }
    }
}

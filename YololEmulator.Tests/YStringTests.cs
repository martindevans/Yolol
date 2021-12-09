using System;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests
{
    [TestClass]
    public class YStringTests
    {
        [TestMethod]
        public void SpanTest()
        {
            var a = "Hello";
            var chars = a.ToArray();
            var sub = System.Runtime.CompilerServices.RuntimeHelpers.GetSubArray<char>(chars, new Range(0, 3));
            sub[0] = 'W';
            Console.WriteLine(new string(chars));

            var cspan = chars.AsMemory()[1..].Span;
            cspan[0] = 'X';
            Console.WriteLine(new string(chars));
        }

        [TestMethod]
        public void YStringToString()
        {
            var s = new YString("abc");
            Assert.AreEqual("abc", s.ToString());
        }

        [TestMethod]
        public void VeryLargeOffset()
        {
            // Create a string longer than ushort.MaxValue
            var a = new YString("a");
            for (var i = 0; i < 16; i++)
                a += a;

            // Subtract off the start, to create an offset into the rope larger than ushort.MaxValue
            var b = (a + "1234567") - a;

            Assert.AreEqual("1234567", b.ToString());
        }

        [TestMethod]
        public void Trim2048()
        {
            var builder = new StringBuilder(2048);
            for (int i = 0; i < 1024; i++)
                builder.Append("*a");

            var slice = new RopeSlice(builder.ToString());

            var trimmed = slice.Trim(1024);

            Assert.AreEqual(1024, trimmed.Length);
        }

        [TestMethod]
        public void Concat1024()
        {
            var y = new YString("*a");
            var x = new YString("");

            for (int i = 0; i < 1024; i++)
                x += y;

            var t = YString.Trim(x, 1024);

            Assert.AreEqual(1024, t.Length);
        }

        [TestMethod]
        public void Concat2048()
        {
            var y = new YString("*a");
            var x = new YString("");

            for (int i = 0; i < 1024; i++)
            {
                x = YString.Add(x, y, 1024);
                Assert.IsTrue(x.Length <= 1024);
            }
        }

        [TestMethod]
        public void ConcatShortened()
        {
            var y = new YString("abcdefghijklmnopqrstuvwxyz");
            var x = y - "defghijklmnopqrstuvw";

            var j = new YString("0123");
            for (int i = 0; i < 3; i++)
                j += x;

            Assert.AreEqual("0123abcxyzabcxyzabcxyz", j.ToString());
        }

        [TestMethod]
        public void ConcatNumber()
        {
            var x = Number.One;
            var y = new YString("234");

            var j = x + y;

            Assert.AreEqual("1234", j.ToString());
        }

        [TestMethod]
        public void ConcatNumberStringOverLengthLimit()
        {
            var result = YString.Add(Number.One, new YString("abcdefghij"), 5);

            Assert.AreEqual("1abcd", result.ToString());
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

        private static string RemoveLast(string haystack, string needle)
        {
            var index = haystack.LastIndexOf(needle);
            if (index >= 0)
                haystack = haystack.Remove(index, needle.Length);

            return haystack;
        }

        [TestMethod]
        public void CombinedFuzz()
        {
            static string RandomString(Random rng, int length)
            {
                const string Numbers = "0123";
                return string.Join("", Enumerable.Range(0, length).Select(_ => Numbers[rng.Next(Numbers.Length)]));
            }

            static Number RandomNumber(Random rng)
            {
                return (Number)rng.Next(0, 100);
            }

            var rng = new Random(7324);
            for (int i = 0; i < 8192; i++)
            {
                var a = RandomString(rng, rng.Next(1, 10));
                var b = RandomString(rng, rng.Next(1, 10));
                var c = RandomString(rng, rng.Next(1, 10));
                var d = RandomString(rng, rng.Next(1, 5));
                var e = RandomString(rng, rng.Next(1, 3));
                var n = RandomNumber(rng);

                var ay = new YString(a);
                var by = new YString(b);
                var cy = new YString(c);
                var dy = new YString(d);
                var ey = new YString(e);

                var final = ay + by + cy - dy - ey - n;

                var abc = a + b + c;

                abc = RemoveLast(abc, d);
                abc = RemoveLast(abc, e);
                abc = RemoveLast(abc, n.ToString());

                Assert.AreEqual(abc, final.ToString());
            }
        }

        [TestMethod]
        public void RemoveFuzz()
        {
            static string RandomString(Random rng, int length)
            {
                const string Numbers = "0123456789";
                return string.Join("", Enumerable.Range(0, length).Select(_ => Numbers[rng.Next(Numbers.Length)]));
            }

            var rng = new Random(345897);
            for (int i = 0; i < 8192; i++)
            {
                // Generate a random string
                var str = RandomString(rng, 20);

                // Take a random chunk in the middle
                var mid = str[rng.Next(str.Length)..];
                mid = mid[..rng.Next(mid.Length)];

                // Subtract that chunk using Yolol semantics
                var haystack = new YString(str);
                var needle = new YString(mid);
                var result = haystack - needle;

                // Subtract it using normal C# operations
                var index = str.LastIndexOf(mid);
                var expected = str.Remove(index, mid.Length);

                Assert.AreEqual(expected, result.ToString());
            }
        }

        [TestMethod]
        public void ConcatFuzz()
        {
            static string RandomString(Random rng, int length)
            {
                const string Numbers = "0123456789";
                return string.Join("", Enumerable.Range(0, length).Select(_ => Numbers[rng.Next(Numbers.Length)]));
            }

            var rng = new Random(7324);
            for (int i = 0; i < 8192; i++)
            {
                var a = RandomString(rng, rng.Next(1, 10));
                var b = RandomString(rng, rng.Next(1, 10));
                var c = RandomString(rng, rng.Next(1, 10));

                var ay = new YString(a);
                var by = new YString(b);
                var cy = new YString(c);

                var final = ay + by + cy;

                Assert.AreEqual(a + b + c, final.ToString());
            }
        }

        [TestMethod]
        public void ConcatNumberFuzz()
        {
            static string RandomString(Random rng, int length)
            {
                const string Numbers = "0123456789";
                return string.Join("", Enumerable.Range(0, length).Select(_ => Numbers[rng.Next(Numbers.Length)]));
            }

            static Number RandomNumber(Random rng)
            {
                return (Number)(rng.NextDouble() * rng.Next());
            }

            var rng = new Random(7324);
            for (int i = 0; i < 8192; i++)
            {
                var a = RandomString(rng, rng.Next(1, 10));
                var b = RandomNumber(rng);

                var ab = new YString(a) + b;

                Assert.AreEqual(a.ToString() + b.ToString(), ab.ToString());
            }
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
        public void SubtractMoreOnesNumber()
        {
            var a = new YString("abc111");
            var c = a - (Number)1111;

            Assert.AreEqual("abc111", c.ToString());
        }

        [TestMethod]
        public void SubtractMoreZeroesNumber()
        {
            var a = new YString("abc000");
            var c = a - (Number)10000;

            Assert.AreEqual("abc000", c.ToString());
        }

        [TestMethod]
        public void SubtractMoreOnes()
        {
            var a = new YString("abc111");
            var c = a - "1111";

            Assert.AreEqual("abc111", c.ToString());
        }

        [TestMethod]
        public void SubtractMoreZeroes()
        {
            var a = new YString("abc000");
            var c = a - "0000";

            Assert.AreEqual("abc000", c.ToString());
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
        public void DecrementSimple()
        {
            var a = new YString("abc");
            a--;

            Assert.AreEqual("ab", a.ToString());
        }

        [TestMethod]
        public void DecrementComplex()
        {
            var a = new YString("abc");
            a -= "a";
            a--;

            Assert.AreEqual("b", a.ToString());
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

        [TestMethod]
        public void TrimShort()
        {
            var str = new YString("Hello");
            var trimmed = YString.Trim(str, 10);

            Assert.AreEqual(str, trimmed);
        }

        [TestMethod]
        public void TrimLong()
        {
            var str = new YString("Hello, Cylon");
            var trimmed = YString.Trim(str, 10);

            Assert.AreEqual("Hello, Cyl", trimmed.ToString());
        }

        [TestMethod]
        public void TrimOnesAndZeroes()
        {
            var str = new YString("ABCD1F0HIJA101");
            var trimmed = YString.Trim(str, 10);

            Assert.AreEqual("ABCD1F0HIJ", trimmed.ToString());
        }
    }
}

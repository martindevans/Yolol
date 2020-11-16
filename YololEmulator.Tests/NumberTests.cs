using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests
{
    [TestClass]
    public class Number6464Tests
    {
        [TestMethod]
        public void IntToNumber()
        {
            var n = (Number)1000;
            Assert.AreEqual((Number)1000, n);
        }

        [TestMethod]
        public void DoubleToNumber()
        {
            var n = (Number)(double)1000;
            Assert.AreEqual(1000, (int)n);
        }

        [TestMethod]
        public void DecimalToNumber()
        {
            var n = (Number)1000m;
            Assert.AreEqual((Number)1000, n);
        }

        [TestMethod]
        public void TruncateOnConstruction()
        {
            var n = (Number)1.234567m;

            Assert.AreEqual((Number)1.234m, n);
            Assert.AreEqual("1.234", n.ToString());
        }

        [TestMethod]
        public void ToStringRemovesDecimalZeros()
        {
            var n = (Number)0.000m;
            Assert.AreEqual("0", n.ToString());
        }

        [TestMethod]
        public void ToStringRemovesDecimalZerosAfterLastDecimal()
        {
            var n = (Number)0.010m;

            Assert.AreEqual(0.010m, (decimal)n);
            Assert.AreEqual("0.01", n.ToString());
        }

        [TestMethod]
        public void ToStringNegative()
        {
            var n = (Number)(-1.020m);
            Assert.AreEqual("-1.02", n.ToString());
        }

        [TestMethod]
        public void ToStringFuzz()
        {
            var rng = new Random(345897);

            for (var i = 0; i < 50000; i++)
            {
                var d = ((decimal)rng.Next() / 1000) * (rng.NextDouble() < 0.5 ? 1 : -1);
                var n = (Number)d;

                var ns = n.ToString();

                Assert.AreEqual(d.ToString(CultureInfo.InvariantCulture), ns);
            }
        }

        [TestMethod]
        public void Equal()
        {
            var a = (Number)3.1415m;

            // ReSharper disable EqualExpressionComparison
#pragma warning disable CS1718 // Comparison made to same variable
            Assert.IsTrue(a == a);
            Assert.IsFalse(a != a);
#pragma warning restore CS1718 // Comparison made to same variable
            // ReSharper restore EqualExpressionComparison

            Assert.IsTrue(a.Equals(a));
            Assert.IsTrue(a.Equals((object)a));

            Assert.AreEqual(a.GetHashCode(), a.GetHashCode());
        }

        [TestMethod]
        public void NotEqual()
        {
            var a = (Number)3.1415m;
            var b = (Number)1;

            // ReSharper disable EqualExpressionComparison
#pragma warning disable CS1718 // Comparison made to same variable
            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
#pragma warning restore CS1718 // Comparison made to same variable
            // ReSharper restore EqualExpressionComparison

            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a.Equals((object)b));

            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [TestMethod]
        public void Subtract()
        {
            var a = (Number)1.010m;
            var b = (Number)19.077m;

            var c = b - a;

            Assert.AreEqual((Number)18.067, c);
        }

        [TestMethod]
        public void Add()
        {
            var a = (Number)1083m;
            var b = (Number)12.77m;

            var c = b + a;
            var e = (Number)1095.770d;

            Assert.AreEqual(e, c);
        }

        [TestMethod]
        public void MethodName()
        {
            var e = (Number)1095.770d;
        }

        [TestMethod]
        public void Multiply()
        {
            var a = (Number)17;
            var b = (Number)9;

            var c = a * b;

            Assert.AreEqual((Number)153, c);
        }

        [TestMethod]
        public void Divide()
        {
            var a = (Number)153;
            var b = (Number)9;

            var c = a / b;

            Assert.AreEqual((Number)17, c);
        }

        [TestMethod]
        public void ModBasic()
        {
            var a = (Number)7;
            var b = (Number)3;

            var c = a % b;

            Assert.AreEqual((Number)1, c);
        }

        [TestMethod]
        public void ModDecimal()
        {
            var a = (Number)7;
            var b = (Number)3.2;

            var c = a % b;

            Assert.AreEqual((Number)0.6, c);
        }

        [TestMethod]
        public void ModNegative()
        {
            var a = (Number)(-7);
            var b = (Number)3;

            var c = a % b;

            Assert.AreEqual((Number)(-1), c);
        }
    }
}

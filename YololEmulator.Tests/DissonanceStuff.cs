using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YololEmulator.Tests
{
    public static class WrappingDelta
    {
        /// <summary>
        /// Wrapping delta for 2 bit numbers
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        internal static int WrappedDelta2(this ushort a, ushort b)
        {
            return WrappedDelta(a, b, 2);
        }

        /// <summary>
        /// Wrapping delta for 7 bit numbers
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        internal static int WrappedDelta7(this ushort a, ushort b)
        {
            return WrappedDelta(a, b, 7);
        }

        /// <summary>
        /// Wrapping delta for 16 bit numbers
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        internal static int WrappedDelta16(this ushort a, ushort b)
        {
            return WrappedDelta(a, b, 16);
        }

        /// <summary>
        /// Calculate what positive value needs to be added to A to get to B
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="bits"></param>
        /// <returns></returns>
        private static int WrappedDelta(this ushort a, ushort b, int bits)
        {
            // Based on: https://stackoverflow.com/questions/44166714/in-c-how-do-i-calculate-the-signed-difference-between-two-48-bit-unsigned-integ

            var mask = (1 << bits) - 1;
            unchecked
            {
                var udiff = b - (uint)a;
                var diff = udiff & mask;
                var idiff = (int)diff;

                if ((udiff & (1 << (bits - 1))) != 0)
                    idiff = -(int)(mask - diff + 1);

                return idiff;
            }
        }
    }

    [TestClass]
    public class Wrapping16
    {
        private static void AssertDelta16(ushort start, int delta)
        {
            var b = unchecked((ushort)(start + delta));
            Assert.AreEqual(delta, start.WrappedDelta16(b));
        }

        [TestMethod]
        public void AssertThat_SlightlyLargerNumber_WrapsToSameValue()
        {
            AssertDelta16(10, 90);
        }

        [TestMethod]
        public void AssertThat_SlightlyLargerNumber_WrapsToSameValue_PastMaxValueThreshold()
        {
            AssertDelta16(ushort.MaxValue - 10, 90);
        }

        [TestMethod]
        public void AssertThat_SlightlySmallerNumber_WrapsToSameValue()
        {
            AssertDelta16(90, -10);
        }

        [TestMethod]
        public void AssertThat_SlightlySmallerNumber_WrapsToSameValue_PastMinValueThreshold()
        {
            AssertDelta16(10, -30);
        }

        [TestMethod]
        public void Fuzz_WrappedDelta_PositiveDelta()
        {
            var seed = new Random().Next();
            Console.WriteLine("Seed: " + seed);
            var rand = new Random(seed);

            for (var i = 0; i < 10000; i++)
            {
                var start = (ushort)rand.Next(ushort.MinValue, ushort.MaxValue / 2);
                var delta = rand.Next(0, ushort.MaxValue / 2);
                AssertDelta16(start, delta);
            }
        }

        [TestMethod]
        public void Fuzz_WrappedDelta_NegativeDelta()
        {
            var seed = new Random().Next();
            Console.WriteLine("Seed: " + seed);
            var rand = new Random(seed);

            for (var i = 0; i < 10000; i++)
            {
                var start = (ushort)rand.Next(ushort.MinValue, ushort.MaxValue / 2);
                var delta = -rand.Next(0, ushort.MaxValue / 2);
                AssertDelta16(start, delta);
            }
        }
    }

    [TestClass]
    public class Wrapping7
    {
        private static void AssertDelta7(ushort start, int delta)
        {
            var b = unchecked((ushort)(start + delta));
            Assert.AreEqual(delta, start.WrappedDelta7(b));
        }

        [TestMethod]
        public void AssertThat_SlightlyLargerNumber_WrapsToSameValue()
        {
            AssertDelta7(10, 20);
        }

        [TestMethod]
        public void AssertThat_SlightlyLargerNumber_WrapsToSameValue_PastMaxValueThreshold()
        {
            AssertDelta7(120, 30);
        }

        [TestMethod]
        public void AssertThat_SlightlySmallerNumber_WrapsToSameValue()
        {
            AssertDelta7(90, -10);
        }

        [TestMethod]
        public void AssertThat_SlightlySmallerNumber_WrapsToSameValue_PastMinValueThreshold()
        {
            AssertDelta7(10, -30);
        }

        [TestMethod]
        public void Fuzz_WrappedDelta_PositiveDelta()
        {
            var seed = new Random().Next();
            Console.WriteLine("Seed: " + seed);
            var rand = new Random(seed);

            for (var i = 0; i < 10000; i++)
            {
                var start = (ushort)rand.Next(byte.MinValue, byte.MaxValue / 4);
                var delta = rand.Next(0, byte.MaxValue / 4);
                AssertDelta7(start, delta);
            }
        }

        [TestMethod]
        public void Fuzz_WrappedDelta_NegativeDelta()
        {
            var seed = new Random().Next();
            Console.WriteLine("Seed: " + seed);
            var rand = new Random(seed);

            for (var i = 0; i < 10000; i++)
            {
                var start = (ushort)rand.Next(byte.MinValue, byte.MaxValue / 4);
                var delta = -rand.Next(0, byte.MaxValue / 4);
                AssertDelta7(start, delta);
            }
        }
    }

    [TestClass]
    public class Wrapping2
    {
        [TestMethod]
        public void Exhaustive0()
        {
            Assert.AreEqual(0, ((ushort)0).WrappedDelta2(0));
            Assert.AreEqual(1, ((ushort)0).WrappedDelta2(1));
            Assert.AreEqual(-2, ((ushort)0).WrappedDelta2(2));
            Assert.AreEqual(-1, ((ushort)0).WrappedDelta2(3));
        }

        [TestMethod]
        public void Exhaustive1()
        {
            Assert.AreEqual(-1, ((ushort)1).WrappedDelta2(0));
            Assert.AreEqual(0, ((ushort)1).WrappedDelta2(1));
            Assert.AreEqual(1, ((ushort)1).WrappedDelta2(2));
            Assert.AreEqual(-2, ((ushort)1).WrappedDelta2(3));
        }

        [TestMethod]
        public void Exhaustive2()
        {
            Assert.AreEqual(-2, ((ushort)2).WrappedDelta2(0));
            Assert.AreEqual(-1, ((ushort)2).WrappedDelta2(1));
            Assert.AreEqual(0, ((ushort)2).WrappedDelta2(2));
            Assert.AreEqual(1, ((ushort)2).WrappedDelta2(3));
        }

        [TestMethod]
        public void Exhaustive3()
        {
            Assert.AreEqual(1, ((ushort)3).WrappedDelta2(0));
            Assert.AreEqual(-2, ((ushort)3).WrappedDelta2(1));
            Assert.AreEqual(-1, ((ushort)3).WrappedDelta2(2));
            Assert.AreEqual(0, ((ushort)3).WrappedDelta2(3));
        }
    }

    [TestClass]
    public class RandomGenerator
    {
        enum UpdateFrequency
        {
            Slow = 60,
            Medium = 61,
            Fast = 62
        }

        [TestMethod]
        public void LCG()
        {
            var a = 6364136223846793005ul;
            var c = 1442695040888963407ul;
            ulong state = 7;

            ulong Next()
            {
                unchecked
                {
                    state = a * state + c;
                }
                return (state >> (int)UpdateFrequency.Fast) + 5;
            }

            var min = Enumerable.Range(0, 100000).Select(_ => Next()).Min();
            var max = Enumerable.Range(0, 100000).Select(_ => Next()).Max();
            Console.WriteLine($"Min: {min} Max:{max}");

            for (var i = 0; i < 1024; i++)
                Console.WriteLine(Next());
        }
    }
}

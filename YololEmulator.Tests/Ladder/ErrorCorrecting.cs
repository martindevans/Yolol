using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class ErrorCorrecting
    {
        private static BitArray GenerateWord(Random rng)
        {
            BitArray bits = new BitArray(8);
            for (var i = 0; i < bits.Length; i++)
                bits[i] = rng.NextDouble() < 0.5;
            return bits;
        }

        private BitArray Hamming8(BitArray input)
        {
            var _ = true;
            BitArray bits = new BitArray(new bool[] {
                _,
                _,
                input[0],
                _,
                input[1],
                input[2],
                input[3],
                _,
                input[4],
                input[5],
                input[6],
                input[7],
            });

            bits[0] = Parity(1, 3, 5, 7, 9, 11);
            bits[1] = Parity(2, 3, 6, 7, 10, 11);
            bits[3] = Parity(4, 5, 6, 7, 12);
            bits[7] = Parity(8, 9, 10, 11, 12);

            return bits;

            bool Parity(params int[] indices)
            {
                var odd = true;
                for (var i = 0; i < indices.Length; i++)
                    odd ^= bits[indices[i] - 1];
                return odd;
            }
        }

        private BitArray SingleBitError(BitArray word, Random rng)
        {
            if (rng.NextDouble() > 0.25)
                return word;

            word[rng.Next(word.Length)] ^= true;
            return word;
        }

        [TestMethod]
        public void GenerateErrorCorrecting()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            // Sanity check hamming code implementation
            //   1  001  1010
            // 0111 0010 1010
            var bits = Hamming8(new BitArray(new[] {true, false, false, true, true, false, true, false}));
            Assert.IsFalse(bits[0]);
            Assert.IsTrue(bits[1]);
            Assert.IsTrue(bits[2]);
            Assert.IsTrue(bits[3]);
            Assert.IsFalse(bits[4]);
            Assert.IsFalse(bits[5]);
            Assert.IsTrue(bits[6]);
            Assert.IsFalse(bits[7]);
            Assert.IsTrue(bits[8]);
            Assert.IsFalse(bits[9]);
            Assert.IsTrue(bits[10]);
            Assert.IsFalse(bits[11]);

            var rng = new Random(23487);
            for (var i = 0; i < 10000; i++)
            {
                var word = GenerateWord(rng);
                var ham8 = Hamming8(word);
                var bad  = SingleBitError(ham8, rng);

                input.Add(new Dictionary<string, Value> {
                    { "i", string.Join("", Stringify(bad)) }
                });
                output.Add(new Dictionary<string, Value> {
                    { "o", string.Join("", Stringify(word)) }
                });
            }
            
            Generator.YololLadderGenerator(input, output);

            static string Stringify(BitArray arr)
            {
                var str = "";
                for (var i = 0; i < arr.Length; i++)
                    str += arr[i] ? "1" : "0";
                return str;
            }
        }
    }
}
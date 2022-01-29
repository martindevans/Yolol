using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class PalindromeNumber
        : BaseGenerator
    {
        public static int Palindromes;

        [TestMethod]
        public void Generate()
        {
            RepeatedFailed = true;
            Run(67343, 25000, true, Generator.ScoreMode.BasicScoring);
        }

        protected override bool GenerateCase(Random random, int index, Dictionary<string, Value> inputs, Dictionary<string, Value> outputs)
        {
            var num = random.Next(250000);
            var palindrome = num.ToString().SequenceEqual(num.ToString().Reverse());

            if (!palindrome && random.NextDouble() > 0.01)
                return false;

            if (palindrome)
                Palindromes++;

            inputs.Add("i", (Number)num);
            outputs.Add("o", (Number)palindrome);
            
            return true;
        }

        protected override void Finalise(List<Dictionary<string, Value>> input, List<Dictionary<string, Value>> output)
        {
            Console.WriteLine(Palindromes);
            base.Finalise(input, output);
        }
    }
}

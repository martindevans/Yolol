using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class BlackFriday
        : BaseGenerator
    {
        [TestMethod]
        public void Generate()
        {
            Run(863499, 25000, true, Generator.ScoreMode.BasicScoring);
        }

        protected override bool GenerateCase(Random random, int index, Dictionary<string, Value> inputs, Dictionary<string, Value> outputs)
        {
            var num = RandomString(random, 10);
            var pin = PinFromNum(num, 4);

            inputs.Add("i", num);
            outputs.Add("o", pin);

            return true;
        }

        private static string PinFromNum(string number, int length)
        {
            var nums = number.Select(a => a - '0').ToList();
            var pin = new List<int>(length);

            var i = nums[0];
            while (pin.Count < 4)
            {
                var p = nums[i % nums.Count];
                pin.Add(p);
                i += p;
            }

            return string.Join("", pin);
        }

        private static readonly string Numbers = "0123456789";

        private static string RandomString(Random rng, int length)
        {
            return string.Join("",
                Enumerable.Range(0, length).Select(_ => RandChar(rng, Numbers))
            );
        }

        private static string RandChar(Random r, string s)
        {
            return s[r.Next(s.Length)].ToString();
        }
    }
}

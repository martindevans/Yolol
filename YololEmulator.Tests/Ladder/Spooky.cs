using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class Spooky
    {
        [TestMethod]
        public void GenerateLeapYear()
        {
            var rng = new Random(57239);
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            void SingleCase(Number i)
            {
                input.Add(new Dictionary<string, Value> {
                    { "i", i },
                });

                output.Add(new Dictionary<string, Value> {
                    { "o", GetSpookiness((int)i) },
                });
            }
            
            SingleCase((Number)1);
            SingleCase((Number)13);
            SingleCase((Number)17);
            SingleCase((Number)99);
            SingleCase((Number)1000);

            for (var x = 0; x < 50; x++)
                SingleCase((Number)rng.Next(0, 1001));

            Generator.YololLadderGenerator(input, output, true);
        }

        private static string GetSpookiness(int age)
        {
            // If the age is evenly divisible by 13: Scary
            if (age % 13 == 0)
                return "Scary";

            // If the year is evenly divisible by 17, go to step 3. Otherwise: Ghastly
            if (age % 17 == 0)
                return "Ghastly";

            // If the year is evenly divisible by 99, go to step 3. Otherwise: Sinister
            if (age % 99 == 0)
                return "Sinister";

            var count = age / 100;
            return "Sp" + new string('o', 2 + count) + "ky";

        }
    }
}

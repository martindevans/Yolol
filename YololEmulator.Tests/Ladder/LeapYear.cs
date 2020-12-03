using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;
using MoreLinq;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class LeapYear
    {
        [TestMethod]
        public void GenerateLeapYear()
        {
            var rng = new Random(57239);
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            void SingleCase(Number i)
            {
                var cosine = i.Cos();

                input.Add(new Dictionary<string, Value> {
                    { "i", i },
                });

                output.Add(new Dictionary<string, Value> {
                    { "o", (Number)IsLeapYear((int)i) },
                });
            }
            
            SingleCase((Number)1900);
            SingleCase((Number)2000);
            SingleCase((Number)2077);
            SingleCase((Number)3000);
            SingleCase((Number)2400);

            for (var x = 0; x < 5000; x++)
                SingleCase((Number)x);

            Generator.YololLadderGenerator(input, output, true);
        }

        bool IsLeapYear(int year)
        {
            // If the year is evenly divisible by 4, go to step 2. Otherwise, not a leap year.
            if (year % 4 != 0)
                return false;

            // If the year is evenly divisible by 100, go to step 3. Otherwise, leap year)
            if (year % 100 != 0)
                return true;

            // If the year is evenly divisible by 100, go to step 3. Otherwise, leap year)
            if (year % 400 != 0)
                return false;

            return true;

        }
    }
}

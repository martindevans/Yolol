using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;
using System.Linq;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class ApocalypticNumbers
    {
        [TestMethod]
        public void GenerateApocalypticNumbers()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            const int CASE_MIN = 2021;
            const int CASE_MAX = 10000;

            var apocalypses = new List<int>();
            for (var i = CASE_MIN; i < CASE_MAX * 1.1; i++)
                if (IsApocalyptic(i))
                    apocalypses.Add(i);

            void SingleCase(int year)
            {
                input.Add(new Dictionary<string, Value> {
                    { "i", (Number)year }
                });

                var next = apocalypses.Cast<int?>().FirstOrDefault(a => a >= year);
                if (!next.HasValue)
                    throw new NotImplementedException(year.ToString());

                output.Add(new Dictionary<string, Value> {
                    { "o", (Number)next },
                });
            }
            
            //SingleCase(2021);
            //SingleCase(2022);
            //SingleCase(2024);
            //SingleCase(2030);
            //SingleCase(2040);

            for (var i = CASE_MIN; i < CASE_MAX; i++)
                SingleCase(i);

            Console.Write($"{apocalypses.Count}/{input.Count}");
            Generator.YololLadderGenerator(input, output, shuffle: false);
        }

        public static bool IsApocalyptic(int num)
        {
            if (!IsLeapYear(num))
                return false;

            if (IsHappyNumber(num))
                return false;

            return true;
        }

        static bool IsLeapYear(int year)
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

        private static readonly HashSet<int> _happy = new HashSet<int>();
        public static bool IsHappyNumber(int num)
        {
            if (num == 0)
                throw new ArgumentOutOfRangeException(nameof(num));

            var input = num;
            while (num != 1 && num != 4)
            {
                if (_happy.Contains(num))
                    return true;
                num = num.ToString().Select(c => c - '0').Select(a => a * a).Sum();
            }

            if (num != 1)
                return false;

            _happy.Add(input);
            return true;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class Persistence
    {
        [TestMethod]
        public void GeneratePersistence()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            static IReadOnlyList<int> Characters(int value)
            {
                return value.ToString().Select(c => c - 48).ToArray();
            }

            static int AdditivePersistence(int value)
            {
                var counter = 0;
                do
                {
                    if (value < 10)
                        break;
                    value = Characters(value).Sum();
                    counter++;
                } while (true);

                return counter;
            }

            static int MultiplicativePersistence(int value)
            {
                var counter = 0;
                do
                {
                    if (value < 10)
                        break;
                    value = Characters(value).Aggregate((a, b) => a * b);
                    counter++;
                } while (true);

                return counter;
            }

            void SingleCase(int number)
            {
                var a = AdditivePersistence(number);
                var b = MultiplicativePersistence(number);
                var o = a == b ? "Neither" : a < b ? "Additive" : "Multiplicative";

                input.Add(new Dictionary<string, Value> { { "i", (Value)number } });
                output.Add(new Dictionary<string, Value> { { "o", o } });
            }

            SingleCase(4);
            SingleCase(1679583);
            SingleCase(123456);
            SingleCase(9999);
            SingleCase(99999);

            for (var i = 0; i < 20000; i++)
                SingleCase(i);

            Generator.YololLadderGenerator(input, output, true, Generator.ScoreMode.BasicScoring);
        }
    }
}

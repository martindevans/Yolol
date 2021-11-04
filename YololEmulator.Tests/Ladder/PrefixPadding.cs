using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class PrefixPadding
    {
        [TestMethod]
        public void GeneratePrefixPadding()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            void SingleCase(int number)
            {
                input.Add(new Dictionary<string, Value> {
                    { "i", (Number)number },
                });

                string result;
                if (number < 0)
                {
                    var abs = Math.Abs((long)number);
                    result = "-" + abs.ToString().PadLeft(10, '0');
                }
                else
                {
                    result = number.ToString().PadLeft(11, '0');
                }

                output.Add(new Dictionary<string, Value> {
                    { "o", result }
                });
            }

            void GenSingleCase(Random rng)
            {
                var num = rng.Next();
                SingleCase(num);
            }

            SingleCase(1);
            SingleCase(-1);
            SingleCase(int.MaxValue);
            SingleCase(int.MinValue);
            SingleCase(12345);

            var rng = new Random(6784);
            for (var i = 0; i < 100000; i++)
                GenSingleCase(rng);

            Generator.YololLadderGenerator(input, output, true);
        }
    }
}

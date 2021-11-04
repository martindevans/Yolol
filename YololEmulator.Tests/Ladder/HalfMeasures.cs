using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class HalfMeasures
    {
        [TestMethod]
        public void GenerateHalfMeasures()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            void SingleCase(int number)
            {
                input.Add(new Dictionary<string, Value> {
                    { "i", new string('*', number * 2) },
                });

                output.Add(new Dictionary<string, Value> {
                    { "o", new string('*', number) }
                });
            }

            SingleCase(1);
            SingleCase(2);
            SingleCase(3);
            SingleCase(4);
            SingleCase(5);

            for (var i = 0; i < 512; i++)
                SingleCase(i);

            Generator.YololLadderGenerator(input, output, true);
        }
    }
}

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;
using System.Linq;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class PresentStacks
    {
        private static readonly int[] Pyramids = {
            0, 1, 5, 14, 30, 55,
            91, 140, 204, 285,
            385, 506, 650, 819,
            1015, 1240, 1496,
            1785, 2109, 2470,
            2870, 3311, 3795,
            4324, 4900, 5525,
            6201, 6930, 7714,
            8555, 9455
        };

        [TestMethod]
        public void GeneratePresentStacks()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            void SingleCase(int presents)
            {
                var stacks = StackCount(presents);

                input.Add(new Dictionary<string, Value> {
                    { "p", (Number)presents }
                });

                output.Add(new Dictionary<string, Value> {
                    { "o", (Number)stacks },
                });
            }
            
            SingleCase(0);
            SingleCase(1);
            SingleCase(5);
            SingleCase(6);
            SingleCase(8);

            for (var i = 0; i < 10000; i++)
                SingleCase(i);

            Generator.YololLadderGenerator(input, output);
        }

        private static int StackCount(int count, int acc = 0)
        {
            if (count == 0)
                return acc;

            var stack = Pyramids.Where(a => a <= count).Max();

            return StackCount(count - stack, acc + 1);
        }
    }
}
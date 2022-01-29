using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;
using System.Linq;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class HappyStacks
    {
        [TestMethod]
        public void GenerateHappyStacks()
        {
            var happy = Enumerable.Range(1, 10000).Where(IsHappyNumber).ToList();

            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            void SingleCase(int presents)
            {
                var stacks = StackCount(presents, happy);

                input.Add(new Dictionary<string, Value> {
                    { "p", (Number)presents }
                });

                output.Add(new Dictionary<string, Value> {
                    { "o", (Number)stacks },
                });
            }
            
            SingleCase(1);
            SingleCase(5);
            SingleCase(6);
            SingleCase(8);
            SingleCase(118);

            for (var i = 0; i < 50000; i++)
                SingleCase(i);

            Generator.YololLadderGenerator(input, output);
        }

        private static int StackCount(int count, List<int> happy, int acc = 0)
        {
            if (count == 0)
                return acc;

            var stack = happy.Where(a => a <= count).Max();

            return StackCount(count - stack, happy, acc + 1);
        }

        public static bool IsHappyNumber(int num)
        {
            while (num != 1 && num != 4)
            {
                num = num.ToString().Select(c => c - '0').Select(a => a * a).Sum();
            }

            return num == 1;
        }
    }
}
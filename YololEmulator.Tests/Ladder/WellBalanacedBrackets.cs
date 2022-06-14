using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class WellBalancedBrackets
    {
        [TestMethod]
        public void GenerateWellBalancedBrackets()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();
            var valid = 0;

            void SingleCase(string str)
            {
                input.Add(new Dictionary<string, Value> {
                    { "i", str },
                });

                var paired = IsPaired(str);
                if (paired)
                    valid++;

                output.Add(new Dictionary<string, Value> {
                    { "o", (Number)paired }
                });
            }

            SingleCase("a");
            SingleCase("(a)");
            SingleCase("(a");
            SingleCase("a)");
            SingleCase("((a)");

            var random = new Random(2345234);
            for (var i = 0; i < 30000; i++)
                SingleCase(GenerateValidString(random, random.Next(3, 8)));
            for (var i = 0; i < 30000; i++)
                SingleCase(GenerateInvalidString(random, random.Next(3, 8)));

            Console.WriteLine(valid);

            Generator.YololLadderGenerator(input, output, true);
        }

        public static bool IsPaired(string input)
        {
            if (input.Length == 0)
                return true;

            var open = 0;

            foreach (var c in input)
            {
                switch (c)
                {
                    case '(':
                        open++;
                        continue;

                    case ')':
                        open--;
                        if (open < 0)
                            return false;
                        continue;

                    default:
                        continue;
                }
            }

            return open == 0;
        }

        private static string GenerateValidString(Random random, int depth)
        {
            if (depth <= 0)
            {
                if (random.NextDouble() < 0.5)
                    return "a";
                return "()";
            }

            switch (random.Next(4))
            {
                case 0: return "a";
                case 1: return $"{GenerateValidString(random, depth - 1)}{GenerateValidString(random, depth - 1)}";
                case 2: return $"({GenerateValidString(random, depth - 1)})";
                case 3: return $"({GenerateValidString(random, depth - 1)})({GenerateValidString(random, depth - 1)})";
            }

            return "";
        }

        private static string GenerateInvalidString(Random random, int depth)
        {
            if (depth <= 0)
            {
                if (random.NextDouble() < 0.5)
                    return "a";
                if (random.NextDouble() < 0.5)
                    return "()";
                if (random.NextDouble() < 0.5)
                    return "(";
                return ")";
            }

            switch (random.Next(11))
            {
                case 0: return "a";
                case 1: return "(";
                case 2: return $"{GenerateInvalidString(random, depth - 1)}{GenerateValidString(random, depth - 1)}";
                case 3: return $"{GenerateValidString(random, depth - 1)}{GenerateInvalidString(random, depth - 1)}";

                case 4: return $"({GenerateInvalidString(random, depth - 1)})";
                case 5: return $"({GenerateValidString(random, depth - 1)}";
                case 6: return $"{GenerateValidString(random, depth - 1)})";

                case 7: return $"({GenerateInvalidString(random, depth - 1)})({GenerateValidString(random, depth - 1)})";
                case 8: return $"({GenerateInvalidString(random, depth - 1)}))({GenerateValidString(random, depth - 1)})";

                case 9: return $"({GenerateValidString(random, depth - 1)})({GenerateInvalidString(random, depth - 1)})";
                case 10: return $")({GenerateValidString(random, depth - 1)})({GenerateInvalidString(random, depth - 1)})";
            }

            return "";
        }
    }
}

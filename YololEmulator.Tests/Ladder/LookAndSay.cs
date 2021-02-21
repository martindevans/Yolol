using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class LookAndSay
    {
        [TestMethod]
        public void GenerateLookAndSay()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            void SingleCase(int value)
            {
                input.Add(new Dictionary<string, Value> {
                    { "i", (Number)value },
                });

                var chars = value.ToString();
                var result = "";

                var current = ' ';
                var counter = 0;
                foreach (var character in value.ToString())
                {
                    if (current == ' ')
                    {
                        counter = 1;
                        current = character;
                    }
                    else if (character != current)
                    {
                        result += counter.ToString();
                        result += current;

                        counter = 1;
                        current = character;
                    }
                    else
                    {
                        counter++;
                    }
                }

                if (current != ' ')
                {
                    result += counter.ToString();
                    result += current;
                }

                output.Add(new Dictionary<string, Value> {
                    { "o", result },
                });
            }

            SingleCase(1);
            SingleCase(11);
            SingleCase(21);
            SingleCase(1211);
            SingleCase(111221);

            // Include some boring numbers
            for (var i = 1; i < 1000; i++)
                SingleCase(i * 23);

            // Attempt to generate some "interesting" cases
            var r = new Random(34857934);
            for (var i = 1; i < 19000; i++)
            {
                var n = "";

                var runs = r.Next(1, 10);
                for (var j = 0; j < runs; j++)
                {
                    var val = r.Next(0, 10).ToString()[0];
                    var len = r.Next(1, 4);

                    n += new string(val, len);
                }

                if (int.TryParse(n, out var result))
                    SingleCase(result);
            }

            Console.WriteLine(output.Count);

            Generator.YololLadderGenerator(input, output);
        }
    }
}
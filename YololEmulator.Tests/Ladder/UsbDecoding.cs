using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;
using System.Linq;
using System.Text;

namespace YololEmulator.Tests.Ladder
{
    /// <summary>
    /// Inspired by https://codegolf.stackexchange.com/questions/229375/decode-usb-packets
    /// </summary>
    [TestClass]
    public class UsbDecoding
    {
        private static string RandomString(Random rng, int minLength, int maxLength)
        {
            const string chars = "JK";

            var length = rng.Next(minLength, maxLength);
            var order = Enumerable.Repeat(chars, length)
                                  .Select(s => chars[rng.Next(chars.Length)])
                                  .ToArray();

            return string.Join("", order);
        }

        private static string? Decode(string encoded)
        {
            var previousInput = 'J';
            var output = new StringBuilder(encoded.Length);

            var outputRunCount = 0;
            foreach (var c in encoded)
            {
                var result = c switch
                {
                    'J' or 'K' => Convert.ToInt32(previousInput == c),
                    _ => throw new NotImplementedException(),
                };
                previousInput = c;

                // Detect bit stuffing
                if (outputRunCount == 6 && result == 0)
                {
                    outputRunCount = 0;
                    continue;
                }

                // Detect invalid bit stuffing
                if (outputRunCount == 6 && result != 0)
                {
                    return null;
                }

                // Keep count of how many 1s have occured in a row
                if (result == 1)
                    outputRunCount++;
                else
                    outputRunCount = 0;

                output.Append(result);
            }

            return output.ToString();
        }

        private static int _ok;
        private static int _err;

        [TestMethod]
        public void GenerateUsbDecoding()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            void SingleCase(string value, bool rejectNonErrors = false)
            {
                var result = Decode(value);
                if (result != null && rejectNonErrors)
                    return;

                if (result == null)
                    _err++;
                else
                    _ok++;

                input.Add(new Dictionary<string, Value> {
                    { "i", new Value(value) },
                });

                output.Add(new Dictionary<string, Value> {
                    { "o", result ?? "error" },
                });
            }

            SingleCase("KJKJKJKJ"); // 00000000
            SingleCase("JJJJJJKKK"); // 11111111
            SingleCase("JKJKKJJ"); // 1000101
            SingleCase("KJJJJJJJJKJ"); // error
            SingleCase("KJJJJJJJKKKKKKKJKJ"); // 0011111111111100

            var rng = new Random(67843576);
            while (input.Count < 7500)
                SingleCase(RandomString(rng, 5, 45), true);
            for (var i = 0; i < 42500; i++)
                SingleCase(RandomString(rng, 5, 45));

            Console.WriteLine($"Ok:{_ok} Err:{_err}");

            Generator.YololLadderGenerator(input, output);
        }
    }
}
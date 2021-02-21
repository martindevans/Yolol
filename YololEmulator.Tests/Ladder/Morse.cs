using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;
using System.Linq;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class Morse
    {
        private static string RandomString(Random rng, int minLength, int maxLength)
        {
            const string chars = "0123456789abcdefghijklmnopqrstuvwxyz ";

            var length = rng.Next(minLength, maxLength);
            var order = Enumerable.Repeat(chars, length)
                                  .Select(s => chars[rng.Next(chars.Length)])
                                  .ToArray();

            return string.Join("", order);
        }

        [TestMethod]
        public void GenerateMorse()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            var morse = InitialiseDictionary();

            void SingleCase(string value)
            {
                input.Add(new Dictionary<string, Value> {
                    { "i", new Value(value) },
                });

                var result = string.Join("", value.Select(c => morse[c]));

                output.Add(new Dictionary<string, Value> {
                    { "o", result },
                });
            }

            SingleCase("hello cylon");
            SingleCase("convert the input into morse code and remove spaces");
            SingleCase("the digits 0 to 9 are included");
            SingleCase("but upper case letters are not");
            SingleCase("good luck");

            var rng = new Random(34598);
            for (var i = 0; i < 10000; i++)
                SingleCase(RandomString(rng, 5, 45));

            Generator.YololLadderGenerator(input, output);
        }

        private static IReadOnlyDictionary<char, string> InitialiseDictionary()
        {
            return new Dictionary<char, string> {
                {'a', ".-"},
                {'b', "-..."},
                {'c', "-.-."},
                {'d', "-.."},
                {'e', "."},
                {'f', "..-."},
                {'g', "--."},
                {'h', "...."},
                {'i', ".."},
                {'j', ".---"},
                {'k', "-.-"},
                {'l', ".-.."},
                {'m', "--"},
                {'n', "-."},
                {'o', "---"},
                {'p', ".--."},
                {'q', "--.-"},
                {'r', ".-."},
                {'s', "..."},
                {'t', "-"},
                {'u', "..-"},
                {'v', "...-"},
                {'w', ".--"},
                {'x', "-..-"},
                {'y', "-.--"},
                {'z', "--.."},
                {'0', "-----"},
                {'1', ".----"},
                {'2', "..---"},
                {'3', "...--"},
                {'4', "....-"},
                {'5', "....."},
                {'6', "-...."},
                {'7', "--..."},
                {'8', "---.."},
                {'9', "----."},
                {' ', ""},
            };
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class DoItYourself
    {
        private static readonly string Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static readonly string Numbers = "0123456789";
        private static readonly string LettersAndNumbers = Letters + Numbers;

        private static string RandChar(Random r, string s)
        {
            return s[r.Next(s.Length)].ToString();
        }

        [TestMethod]
        public void GeneratePostCodes()
        {
            var rng = new Random(678234);
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            // AAA BBB
            // ^ Outward code
            //     ^ Inward code
            //
            // Outcode:
            //  - 2-4 characters long
            //  - Always begins with a letter
            //  - May end with a number or letter
            //
            // Incode:
            //  - Exactly 3 characters long
            //  - Always begins with a number

            bool Validate(string code)
            {
                var parts = code.Split(" ");
                if (parts.Length != 2)
                    return false;

                var outcode = parts[0];
                if (outcode.Length < 2 || outcode.Length > 4)
                    return false;
                if (!char.IsLetter(outcode[0]))
                    return false;
                if (!outcode.All(char.IsLetterOrDigit))
                    return false;

                var incode = parts[1];
                if (incode.Length != 3)
                    return false;
                if (!char.IsNumber(incode[0]))
                    return false;

                return true;
            }

            string GenerateValidOutcode(Random rng)
            {
                var r = RandChar(rng, Letters) + RandChar(rng, LettersAndNumbers);

                if (rng.NextDouble() < 0.5f)
                    r += RandChar(rng, LettersAndNumbers);
                if (rng.NextDouble() < 0.5f)
                    r += RandChar(rng, LettersAndNumbers);

                return r;
            }

            string GenerateValidIncode(Random rng)
            {
                return RandChar(rng, Numbers)
                     + RandChar(rng, LettersAndNumbers)
                     + RandChar(rng, LettersAndNumbers);
            }

            string GenerateInvalidOutcode(Random rng)
            {
                var r = RandChar(rng, LettersAndNumbers) + RandChar(rng, LettersAndNumbers);

                if (rng.NextDouble() < 0.5f)
                    r += RandChar(rng, LettersAndNumbers);
                if (rng.NextDouble() < 0.5f)
                    r += RandChar(rng, LettersAndNumbers);
                if (rng.NextDouble() < 0.5f)
                    r += RandChar(rng, LettersAndNumbers);
                if (rng.NextDouble() < 0.5f)
                    r += RandChar(rng, LettersAndNumbers);

                return r;
            }

            string GenerateInvalidIncode(Random rng)
            {
                var r = RandChar(rng, LettersAndNumbers)
                     + RandChar(rng, LettersAndNumbers)
                     + RandChar(rng, LettersAndNumbers);

                if (rng.NextDouble() < 0.5f)
                    r += RandChar(rng, LettersAndNumbers);
                if (rng.NextDouble() < 0.5f)
                    r += RandChar(rng, LettersAndNumbers);

                return r;
            }

            string GenerateValidCode(Random rng) => $"{GenerateValidOutcode(rng)} {GenerateValidIncode(rng)}";

            string GenerateInvalidCode(Random rng) =>
                rng.Next(0, 5) switch {
                    0 => $"{GenerateInvalidOutcode(rng)} {GenerateValidIncode(rng)}",
                    1 => $"{GenerateValidOutcode(rng)} {GenerateInvalidIncode(rng)}",
                    2 => $"{GenerateInvalidOutcode(rng)} {GenerateInvalidIncode(rng)}",
                    3 => $"{GenerateValidIncode(rng)} {GenerateValidOutcode(rng)}",
                    4 => string.Join("", Enumerable.Range(0, rng.Next(5)).Select(a => RandChar(rng, LettersAndNumbers)).ToList()),
                    _ => " "
                };


            var valid = 0;
            var invalid = 0;
            void SingleCase(string code)
            {
                var v = Validate(code);
                if (v)
                    valid++;
                else
                    invalid++;

                input.Add(new Dictionary<string, Value> {
                    { "c", code }
                });

                output.Add(new Dictionary<string, Value> {
                    { "o", (Number)v }
                });
            }

            SingleCase("CYL ON");
            SingleCase("AAA AAA");
            SingleCase("AA1 1AA");
            SingleCase("AZA9 7CX");
            SingleCase("A11 111");

            for (var x = 0; x < 25000; x++)
            {
                SingleCase(GenerateInvalidCode(rng));
                SingleCase(GenerateValidCode(rng));
            }

            //Console.WriteLine(valid);
            //Console.WriteLine(invalid);
            Generator.YololLadderGenerator(input, output, true, Generator.ScoreMode.BasicScoring, Generator.YololChip.Professional);
        }
    }
}

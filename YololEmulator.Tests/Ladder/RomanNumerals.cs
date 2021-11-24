using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class RomanNumerals
        : BaseGenerator
    {
        [TestMethod]
        public void Generate()
        {
            Run(68567, 3998, true, Generator.ScoreMode.BasicScoring);
        }

        protected override bool GenerateCase(Random random, int index, Dictionary<string, Value> inputs, Dictionary<string, Value> outputs)
        {
            index += 1;
            inputs.Add("i", (Number)index);
            outputs.Add("o", ToRoman(index));

            return true;
        }

        public static string ToRoman(int number)
        {
            return number switch {
                > 3999 => throw new ArgumentOutOfRangeException(nameof(number), number, null),
                >= 1000 => "M" + ToRoman(number - 1000),
                >= 900 => "CM" + ToRoman(number - 900),
                >= 500 => "D" + ToRoman(number - 500),
                >= 400 => "CD" + ToRoman(number - 400),
                >= 100 => "C" + ToRoman(number - 100),
                >= 90 => "XC" + ToRoman(number - 90),
                >= 50 => "L" + ToRoman(number - 50),
                >= 40 => "XL" + ToRoman(number - 40),
                >= 10 => "X" + ToRoman(number - 10),
                >= 9 => "IX" + ToRoman(number - 9),
                >= 5 => "V" + ToRoman(number - 5),
                >= 4 => "IV" + ToRoman(number - 4),
                >= 1 => "I" + ToRoman(number - 1),
                0 => "",
                _ => throw new ArgumentOutOfRangeException(nameof(number), number, null)
            };
        }
    }
}

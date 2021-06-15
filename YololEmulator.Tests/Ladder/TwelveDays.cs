using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class TwelveDays
    {
        [TestMethod]
        public void GenerateTwelveDaysOfXmas()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            var index = 0;
            foreach (var line in TwelveDaysOfChristmas())
            {
                index++;

                input.Add(new Dictionary<string, Value> {
                    { "i", (Value)index },
                });

                output.Add(new Dictionary<string, Value> {
                    { "o", line },
                });
            }

            Generator.YololLadderGenerator(input, output, false);
        }

        private IEnumerable<string> TwelveDaysOfChristmas()
        {
            var lines = new[] {
                "Twelve Drummers Drumming,",
                "Eleven Pipers Piping,",
                "Ten Lords-a-Leaping,",
                "Nine Ladies Dancing,",
                "Eight Maids-a-Milking,",
                "Seven Swans-a-Swimming,",
                "Six Geese-a-Laying,",
                "Five Gold Rings,",
                "Four Calling Birds,",
                "Three French Hens,",
                "Two Turtle Doves, and",
                "A Partridge in a Pear Tree."
            };

            for (var i = 1; i <= 12; i++)
            {
                yield return $"On the {NumToWord(i)} day of Christmas";
                yield return "My true love sent to me";

                for (var j = i; j > 0; j--)
                    yield return lines[^j];
            }
        }

        private static string NumToWord(int value)
        {
            return value switch {
                1 => "First",
                2 => "Second",
                3 => "Third",
                4 => "Fourth",
                5 => "Fifth",
                6 => "Sixth",
                7 => "Seventh",
                8 => "Eighth",
                9 => "Ninth",
                10 => "Tenth", 
                11 => "Eleventh",
                12 => "Twelfth",
                _ => throw new NotImplementedException(value.ToString()),
            };
        }
    }
}

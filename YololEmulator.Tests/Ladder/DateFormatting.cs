using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class DateFormatting
    {
        [TestMethod]
        public void GenerateDateFormatting()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            void SingleCase(int value)
            {
                static string AddOrdinal(int num)
                {
                    if( num <= 0 ) return num.ToString();

                    switch(num)
                    {
                        case 11:
                        case 12:
                        case 13:
                            return num + "th";
                    }
    
                    switch(num % 10)
                    {
                        case 1:
                            return num + "st";
                        case 2:
                            return num + "nd";
                        case 3:
                            return num + "rd";
                        default:
                            return num + "th";
                    }
                }

                var d = new DateTime(2021, 1, 1) + TimeSpan.FromDays(value);
                var day = AddOrdinal(d.Day);
                var mon = d.ToString("MMM", CultureInfo.InvariantCulture);

                input.Add(new Dictionary<string, Value> { { "a", (Number)value } });
                output.Add(new Dictionary<string, Value> { { "o", day + " " + mon } });
            }

            SingleCase(0);
            SingleCase(17);
            SingleCase(23);
            SingleCase(126);
            SingleCase(364);

            for (var x = 0; x < 365; x++)
                SingleCase(x);

            Generator.YololLadderGenerator(input, output, true, Generator.ScoreMode.BasicScoring);
        }
    }
}

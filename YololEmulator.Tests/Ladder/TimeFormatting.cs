using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class TimeFormatting
    {
        [TestMethod]
        public void GenerateTimeFormatting()
        {
            var rng = new Random(57239);
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            void SingleCase(int seconds)
            {
                var i = seconds;

                var years = seconds / 31536000;
                seconds -= years * 31536000;

                var days = seconds / 86400;
                seconds -= days * 86400;

                var hours = seconds / 3600;
                seconds -= hours * 3600;

                var minutes = seconds / 60;
                seconds -= minutes * 60;

                var parts = new List<string>();

                if (years > 0)
                    parts.Add(Pluralise(years, "year"));
                if (days > 0)
                    parts.Add(Pluralise(days, "day"));
                if (hours > 0)
                    parts.Add(Pluralise(hours, "hour"));
                if (minutes > 0)
                    parts.Add(Pluralise(minutes, "minute"));
                if (seconds > 0)
                    parts.Add(Pluralise(seconds, "second"));

                var o = Output(parts.ToArray());

                input.Add(new Dictionary<string, Value> { { "a", (Value)i } });
                output.Add(new Dictionary<string, Value> { { "o", o } });
            }

            SingleCase(1);
            SingleCase(62);
            SingleCase(3664);
            SingleCase(86400);
            SingleCase(282999961);

            for (var x = 0; x < 4000; x++)
            {
                var i = rng.Next(1, 283000000);
                SingleCase(i);
            }

            Generator.YololLadderGenerator(input, output, true, Generator.ScoreMode.BasicScoring);
        }

        private static string Output(string[] parts)
        {
            if (parts.Length == 1)
                return parts[0];

            return string.Join(", ", parts[0..^1]) + " and " + parts.Last();
        }

        private static string Pluralise(int amount, string unit)
        {
            return $"{amount} {unit}" + (amount > 1 ? "s" : "");
        }
    }
}

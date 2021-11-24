using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class Macbeth
    {
        [TestMethod]
        public async Task GenerateMacbeth()
        {
            string contents;
            using (var wc = new HttpClient())
                contents = await wc.GetStringAsync("https://raw.githubusercontent.com/cs109/2015/f4dcbcc1446b7dfc33ecad4dd5e92b9a23a274e0/Lectures/Lecture15b/sparklect/shakes/macbeth.txt");

            static char? Clean(char c)
            {
                c = char.ToLowerInvariant(c);
                if (char.IsLetterOrDigit(c))
                    return c;

                if (c == '\'')
                    return null;

                return ' ';
            }

            var characters = (from cc in contents
                              let c = Clean(cc)
                              where c.HasValue
                              select c.ToString()).ToList();

            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            var result = new StringBuilder();
            var counter = 0;
            var prev = "";
            foreach (var character in characters)
            {
                if (character == " " && prev == " ")
                    continue;

                input.Add(new Dictionary<string, Value> {
                    { "i", (Number)counter },
                    { "p", prev },
                });
                prev = character;

                output.Add(new Dictionary<string, Value> {
                    { "o", character },
                });

                result.Append(character);
                counter++;
            }

            Generator.YololLadderGenerator(input, output, shuffle: false, mode: Generator.ScoreMode.Approximate);
            Console.WriteLine(result);
        }
    }
}
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
    public class FelizNavidad
    {
        [TestMethod]
        public async Task GenerateLyrics()
        {
            string contents;
            using (var wc = new HttpClient())
                contents = await wc.GetStringAsync("https://gist.githubusercontent.com/martindevans/b4d0c298683258cb86bf7809e84db3bf/raw/a1978f25de4df88220ee381fc6271d4cc6f07774/FelizNavidad");

            var characters = contents.Select(a => a.ToString()).ToList();

            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            var result = new StringBuilder();
            var counter = 0;
            foreach (var character in characters)
            {
                input.Add(new Dictionary<string, Value> {
                    { "i", (Number)counter },
                });

                output.Add(new Dictionary<string, Value> {
                    { "o", character },
                });

                result.Append(character);
                counter++;
            }

            Generator.YololLadderGenerator(input, output, shuffle: false, mode: Generator.ScoreMode.BasicScoring);
        }
    }
}
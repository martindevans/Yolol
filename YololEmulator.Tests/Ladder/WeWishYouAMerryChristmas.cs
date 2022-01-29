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
    public class WeWishYouAMerryChristmas
    {
        [TestMethod]
        public async Task GenerateLyrics()
        {
            string contents;
            using (var wc = new HttpClient())
                contents = await wc.GetStringAsync("https://gist.githubusercontent.com/martindevans/a32bf96c2117b98ba786ae77f1ca8651/raw/926178438c94807c9078d42aead45adbf94590e2/WeWishYouAMerryChristmas");

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
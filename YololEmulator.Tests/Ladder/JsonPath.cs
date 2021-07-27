using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class JsonPath
    {
        private const string Chars = "abcdefghijklmnopqrstuvwxyz";

        private static string? GeneratePath(Random random, JToken token)
        {
            if (token is JArray arr)
            {
                return $"[{random.Next(arr.Count)}]";
            }
            else if (token is JObject jobj)
            {
                var props = jobj.Properties().ToList();
                var prop = props[random.Next(props.Count)];
                return $"{prop.Name}.{GeneratePath(random, prop.Value)}";
            }
            else
                return null;
        }

        private static JToken GenerateJObj(Random random, int depthLimit = 7)
        {
            if (depthLimit == 0 || random.NextDouble() < 1f/depthLimit - 0.1)
            {
                if (random.NextDouble() < 0.5)
                    return JToken.FromObject(Chars[random.Next(Chars.Length)].ToString());
                else
                    return JToken.FromObject(random.Next(1000));
            }

            if (random.NextDouble() < 0.2)
            {
                var jarr = new JArray();
                for (var i = 0; i < 1 + random.Next(5); i++)
                    jarr.Add(Chars[random.Next(Chars.Length)].ToString());
                return jarr;
            }

            var j = new JObject();

            var keys = new HashSet<string>();
            for (var i = 0; i < 1 + random.Next(5); i++)
            {
                string key;
                do
                {
                    key = random.Next(0, 1000).ToString();
                } while (keys.Contains(key));
                keys.Add(key);

                j[key] = GenerateJObj(random, depthLimit - 1);
            }

            return j;
        }

        [TestMethod]
        public void GenerateJsonPath()
        {
            var rng = new Random(6704);
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            void SingleCase(JToken j, string path)
            {
                input.Add(new Dictionary<string, Value> {
                    { "i", j.ToString(Newtonsoft.Json.Formatting.None).Replace("\"", "'") },
                    { "p", path }
                });

                var jt =  j.SelectToken(path);
                if (jt == null)
                    return;

                Value ov;
                if (jt.Type == JTokenType.String)
                    ov = jt.ToString();
                else if (jt.Type == JTokenType.Integer)
                    ov = (Value)(int)jt;
                else
                    return;

                output.Add(new Dictionary<string, Value> {
                    { "o", ov },
                });
            }

            SingleCase(JToken.Parse("{'a':1}"), "a");
            SingleCase(JToken.Parse("{'a':1,'b':2}"), "b");
            SingleCase(JToken.Parse("{'a':1,'b':{'a':1}}"), "b.a");
            SingleCase(JToken.Parse("{'a':[1,2,3]}"), "a.[1]");
            SingleCase(JToken.Parse("{'a':1,'b':{'a':[1,2,'z']}}"), "b.a.[2]");

            for (var x = 0; x < 10000; x++)
            {
                var obj = GenerateJObj(rng);
                var path = GeneratePath(rng, obj)?.TrimEnd('.');
                if (path != null)
                    SingleCase(obj, path);
            }

            Generator.YololLadderGenerator(input, output, true, Generator.ScoreMode.BasicScoring);
        }
    }
}

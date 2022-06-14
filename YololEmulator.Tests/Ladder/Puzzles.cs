using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class Puzzles
    {
        private static string RandomString(Random rng, int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[rng.Next(s.Length)]).ToArray());
        }

        [TestMethod]
        public void Intersection()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            var rng = new Random(234523);
            char RandChar()
            {
                return (char)('a' + rng.Next(0, 26));
            }

            for (var i = 0; i < 200; i++)
            {
                var l = string.Join("", Enumerable.Range(0, 10).Select(_ => RandChar()));
                var r = string.Join("", Enumerable.Range(0, 10).Select(_ => RandChar()));

                var intersection = string.Join("", l.Intersect(r));

                input.Add(new Dictionary<string, Value> {{"l", l}, {"r", r}});
                output.Add(new Dictionary<string, Value> {{"i", intersection}});
            }

            Generator.YololLadderGenerator(input, output);
        }

        [TestMethod]
        public void Union()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            var rng = new Random(234523);
            char RandChar()
            {
                return (char)('a' + rng.Next(0, 26));
            }

            for (var i = 0; i < 200; i++)
            {
                var l = string.Join("", Enumerable.Range(0, 10).Select(_ => RandChar()));
                var r = string.Join("", Enumerable.Range(0, 10).Select(_ => RandChar()));

                var union = string.Join("", l.Union(r));

                input.Add(new Dictionary<string, Value> {{"l", l}, {"r", r}});
                output.Add(new Dictionary<string, Value> {{"i", union}});
            }

            Generator.YololLadderGenerator(input, output);
        }

        [TestMethod]
        public void Parsing()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            var rng = new Random(234523);

            for (var i = 0; i < 200; i++)
            {
                var v = (Value)rng.Next(0, 100000);
                v /= Value.Exponent((Number)10, (Number)rng.Next(0, 3));

                input.Add(new Dictionary<string, Value> { { "i", v.ToString() } });
                output.Add(new Dictionary<string, Value> {{ "o", v }});
            }

            Generator.YololLadderGenerator(input, output);
        }

        [TestMethod]
        public void BaseConvert()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            var rng = new Random(234523);

            for (var i = 0; i < 200; i++)
            {
                var n = new byte[rng.Next(5, 20)];
                rng.NextBytes(n);
                var inputstr = BitConverter.ToString(n).Replace("-","");
                var outputstr = Convert.ToBase64String(n);

                input.Add(new Dictionary<string, Value> { { "i", inputstr } });
                output.Add(new Dictionary<string, Value> {{ "o", outputstr }});
            }

            Generator.YololLadderGenerator(input, output);
        }

        [TestMethod]
        public void LogicalAwesome()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            var rng = new Random(987234);

            for (var i = 0; i < 200; i++)
            {
                Value a = (Value)(rng.NextDouble() < 0.5f ? 1 : 0);
                Value b = (Value)(rng.NextDouble() < 0.5f ? 1 : 0);

                var op = rng.Next(0, 3) switch
                {
                    0 => "or",
                    1 => "and",
                    2 => "xor",
                    3 => "nor",
                    4 => "nand",
                    5 => "eq",
                    6 => "neq",
                    _ => throw new NotImplementedException()
                };

                var result = op switch
                {
                    "or" => a | b,
                    "and" => a & b,
                    "xor" => (a | b) && !(a & b), 
                    "nor" => !(a | b),
                    "nand" => !(a & b),
                    "eq" => a == b,
                    "neq" => a != b,
                    _ => throw new NotImplementedException()
                };

                input.Add(new Dictionary<string, Value> { { "a", a }, { "b", b }, { "op", op } });
                output.Add(new Dictionary<string, Value> {{ "o", new Value(result) }});
            }

            Generator.YololLadderGenerator(input, output);
        }

        [TestMethod]
        public void LogicalAwesome2()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            var rng = new Random(987345);

            for (var i = 0; i < 1000; i++)
            {
                Value a = (Value)(rng.NextDouble() < 0.5f ? 1 : 0);
                Value b = (Value)(rng.NextDouble() < 0.5f ? 1 : 0);

                var opi = rng.Next(0, 10);
                var op = opi switch
                {
                    0 => "or",
                    1 => "and",
                    2 => "xor",
                    3 => "nor",
                    4 => "nand",
                    5 => "eq",
                    6 => "neq",
                    7 => "xnor",
                    8 => "true",
                    9 => "false",
                    _ => throw new NotImplementedException()
                };

                var result = op switch
                {
                    "or" => a | b,
                    "and" => a & b,
                    "xor" => (a | b) & !(a & b), 
                    "nor" => !(a | b),
                    "nand" => !(a & b),
                    "eq" => a == b,
                    "neq" => a != b,
                    "xnor" => !((a | b) & !(a & b)),
                    "true" => true,
                    "false" => false,
                    _ => throw new NotImplementedException()
                };

                input.Add(new Dictionary<string, Value> { { "a", a }, { "b", b }, { "op", op } });
                output.Add(new Dictionary<string, Value> {{ "o", new Value(result) }});
            }

            Generator.YololLadderGenerator(input, output);
        }

        [TestMethod]
        public void Atan2_Impossible()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            var rng = new Random(349087);

            for (var i = 0; i < 400; i++)
            {
                var a = (Number)(rng.NextDouble() * 20);
                var b = (Number)(rng.NextDouble() * 20);
                if (a == Number.Zero && b == Number.Zero)
                    continue;

                var c = Math.Atan2((float)a, (float)b);
                var cn = (Number)(decimal)c;

                input.Add(new Dictionary<string, Value> { { "a", a }, { "b", b } });
                output.Add(new Dictionary<string, Value> {{ "o", cn }});
            }

            Generator.YololLadderGenerator(input, output);
        }

        [TestMethod]
        public void BasicSorting()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            var rng = new Random(8672);
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            for (var x = 0; x < 800; x++)
            {
                
                var i = new string(Enumerable.Repeat(chars, rng.Next(1, 10)).Select(s => s[rng.Next(s.Length)]).ToArray());
                var o = string.Concat(i.OrderBy(a => a));

                input.Add(new Dictionary<string, Value> { { "i", i } });
                output.Add(new Dictionary<string, Value> { { "o", o } });
            }

            Generator.YololLadderGenerator(input, output);
        }

        [TestMethod]
        public void ReallyLongSorting()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            var rng = new Random(8672);
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ abcdefghijklmnopqrstuvwxyz 1234567890";

            void SingleCase(string i)
            {
                var o = string.Concat(i.OrderBy(a => a));

                input.Add(new Dictionary<string, Value> { { "i", i } });
                output.Add(new Dictionary<string, Value> { { "o", o } });
            }

            SingleCase("Hello Cylon");
            SingleCase("Sorting");
            SingleCase("With really long strings");
            SingleCase("Up to 1024 chars");
            SingleCase("Good Luck");

            for (var x = 0; x < 10000; x++)
            {
                var i = new string(Enumerable.Repeat(chars, rng.Next(1, 1024)).Select(s => s[rng.Next(s.Length)]).ToArray());
                SingleCase(i);
            }

            Generator.YololLadderGenerator(input, output);
        }

        [TestMethod]
        public void NyefariTapeSorting()
        {
            var rng = new Random(234512);
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            for (var i = 0; i < 800; i++)
            {
                var a = RandomString(rng, rng.Next(1, 6));
                var b = RandomString(rng, rng.Next(1, 6));
                var c = RandomString(rng, rng.Next(1, 6));
                var d = RandomString(rng, rng.Next(1, 6));
                var e = RandomString(rng, rng.Next(1, 6));

                var inputList = new List<string> {
                    "<" + a + ">",
                    "<" + b + ">",
                    "<" + c + ">",
                    "<" + d + ">",
                    "<" + e + ">"
                };

                var outputList = new List<string>();
                outputList.AddRange(inputList);
                outputList.Sort(StringComparer.OrdinalIgnoreCase);

                var inputString = string.Join("", inputList);
                var outputString = string.Join("", outputList);

                input.Add(new Dictionary<string, Value> { { "i", inputString } });
                output.Add(new Dictionary<string, Value> { { "o", outputString } });

            }

            Generator.YololLadderGenerator(input, output);
        }

        [TestMethod]
        public void NyefariTapeSorting2()
        {
            var rng = new Random(3645);
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            for (var i = 0; i < 2000; i++)
            {
                var records = new List<string>();
                for (var j = 0; j < 100; j++)
                    records.Add($"<{RandomString(rng, 5)}>");

                var inputString = string.Join("", records);
                records.Sort(Compare);
                var outputString = string.Join("", records);

                if (inputString.Length > 1024 || outputString.Length > 1024)
                    throw new InvalidOperationException("String too long!");

                input.Add(new Dictionary<string, Value> { { "i", inputString } });
                output.Add(new Dictionary<string, Value> { { "o", outputString } });

            }

            Generator.YololLadderGenerator(input, output);

            int Compare(string a, string b)
            {
                var ay = new YString(a);
                var by = new YString(b);

                if (ay < by)
                    return -1;
                if (ay > by)
                    return 1;
                return 0;
            }
        }

        [TestMethod]
        public void BasicInventoryManagement()
        {
            var inventory = new List<char>();

            var rng = new Random(234859);
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            for (var x = 0; x < 1000; x++)
            {
                // Generate a new set of stuff to add to the warehouse
                var i = RandomString(rng, rng.Next(0, 30));
                inventory.AddRange(i);

                // Generate a request from the warehouse
                var r = RandomString(rng, rng.Next(0, 5));
                input.Add(new Dictionary<string, Value> { { "i", i }, { "r", r } });

                // Filter request by what can actually be provided
                var actual = new string(r.Where(c => inventory.Remove(c)).ToArray());

                output.Add(new Dictionary<string, Value> { { "o", actual } });
            }

            Generator.YololLadderGenerator(input, output, false);
        }

        [TestMethod]
        public void Min10()
        {
            var rng = new Random(8734578);
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            for (var x = 0; x < 2000; x++)
            {
                // Generate a new set of numbers
                var numbers = Enumerable.Range(0, 10).Select(_ => rng.Next(-5000, 5000)).ToArray();
                var min = numbers.Min();

                input.Add(new Dictionary<string, Value> {
                    { "a", (Value)numbers[0] },
                    { "b", (Value)numbers[1] },
                    { "c", (Value)numbers[2] },
                    { "d", (Value)numbers[3] },
                    { "e", (Value)numbers[4] },
                    { "f", (Value)numbers[5] },
                    { "g", (Value)numbers[6] },
                    { "h", (Value)numbers[7] },
                    { "i", (Value)numbers[8] },
                    { "j", (Value)numbers[9] },
                });

                output.Add(new Dictionary<string, Value> { { "o", (Value)min } });
            }

            Generator.YololLadderGenerator(input, output, true);
        }

        [TestMethod]
        public void CaesarCipher()
        {
            string Cipher(string input, int key)
            {
                return string.Join("", input.Select(c => (char)((c + key - 'A') % 26 + 'A')));
            }

            var rng = new Random(8734578);
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            for (var x = 0; x < 1500; x++)
            {
                // Generate a new set of numbers
                var key = rng.Next(0, 20);
                var str = RandomString(rng, rng.Next(5, 50));
                var enc = Cipher(str, key);

                input.Add(new Dictionary<string, Value> { { "s", str}, { "k", (Value)key } });
                output.Add(new Dictionary<string, Value> { { "c", enc } });
            }

            Generator.YololLadderGenerator(input, output, true);
        }

        [TestMethod]
        public void ApproxAtan()
        {
            var rng = new Random(8587569);
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            for (var x = 0; x < 1000; x++)
            {
                var a = (Number)(rng.NextDouble() * 100 - 50);
                var b = (Number)(rng.NextDouble() * 100 - 50);
                var c = Math.Atan2((double)a, (double)b);

                input.Add(new Dictionary<string, Value> { { "a", a }, { "b", b } });
                output.Add(new Dictionary<string, Value> { { "c", (Number)c } });
            }

            Generator.YololLadderGenerator(input, output, true, Generator.ScoreMode.Approximate);
        }

        [TestMethod]
        public void Xor16()
        {
            var rng = new Random(6456533);
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            checked
            {
                for (var x = 0; x < 1000; x++)
                {

                    var a = (ushort)rng.Next(0, ushort.MaxValue);
                    var b = (ushort)rng.Next(0, ushort.MaxValue);
                    var c = (ushort)(a ^ b);

                    input.Add(new Dictionary<string, Value> {{ "a", new Value((Number)a) }, { "b", new Value((Number)b) }});
                    output.Add(new Dictionary<string, Value> {{"o", new Value((Number)c) }});
                }
            }

            Generator.YololLadderGenerator(input, output);
        }

        [TestMethod]
        public void And16()
        {
            var rng = new Random(6456533);
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            checked
            {
                for (var x = 0; x < 50000; x++)
                {

                    var a = (ushort)rng.Next(0, ushort.MaxValue);
                    var b = (ushort)rng.Next(0, ushort.MaxValue);
                    var c = (ushort)(a & b);

                    input.Add(new Dictionary<string, Value> { { "a", new Value((Number)a) }, { "b", new Value((Number)b) } });
                    output.Add(new Dictionary<string, Value> { { "o", new Value((Number)c) } });
                }
            }

            Generator.YololLadderGenerator(input, output);
        }

        [TestMethod]
        public void LongSorting()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            var rng = new Random(873445);
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            for (var x = 0; x < 400; x++)
            {
                var i = new string(Enumerable.Repeat(chars, 128).Select(s => s[rng.Next(s.Length)]).ToArray());
                var o = string.Concat(i.OrderBy(a => a));
                input.Add(new Dictionary<string, Value> { { "i", i } });
                output.Add(new Dictionary<string, Value> { { "o", o } });
            }

            input.Add(new Dictionary<string, Value> { { "i", "00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000" } });
            output.Add(new Dictionary<string, Value> { { "o", "00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000" } });

            Generator.YololLadderGenerator(input, output);
        }

        [TestMethod]
        public void ApproxLog2()
        {
            var rng = new Random(349710);
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            for (var x = 0; x < 2000; x++)
            {
                var a = (Number)(rng.NextDouble() * 10000);
                if (a == Number.Zero)
                    a = (Number)0.001;

                var c = Math.Round(Math.Log2((double)a), Number.Decimals);

                input.Add(new Dictionary<string, Value> { { "a", a } });
                output.Add(new Dictionary<string, Value> { { "o", (Number)c } });
            }

            Generator.YololLadderGenerator(input, output, true, Generator.ScoreMode.Approximate);
        }

        [TestMethod]
        public void FizzBuzz()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            for (var x = 1; x < 2000; x++)
            {
                var r = ((x % 3 == 0, x % 5 == 0)) switch
                {
                    (true, true) => "FizzBuzz",
                    (true, false) => "Fizz",
                    (false, true) => "Buzz",
                    (false, false) => (Value)x,
                };

                output.Add(new Dictionary<string, Value> { { "o", r } });
            }

            Generator.YololLadderGenerator(input, output, false);
        }
    }
}

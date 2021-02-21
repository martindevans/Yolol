using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class WhatsThatOperator
    {
        [TestMethod]
        public void GenerateWhatsThatOperator()
        {
            var rng = new Random(568354);
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            void SingleCase(Number a, string b, Number c, string d, Number e)
            {
                static string Clean(string op)
                {
                    if (op.Contains("?"))
                        return "";
                    return op;
                }

                static Number Accumulate(Number a, string op, Number b)
                {
                    switch (op.Replace("?", ""))
                    {
                        case "+": return a + b;
                        case "*": return a * b;
                        case "/": return a / b;
                        case "-": return a - b;
                        case "^": return a.Exponent(b);
                    }

                    throw new NotImplementedException(op);
                }

                var m = new[] { b, d }
                        .Single(x => x.Contains("?"))
                        .Replace("?", "");

                var r = a;
                r = Accumulate(r, b, c);
                r = Accumulate(r, d, e);

                input.Add(new Dictionary<string, Value> {
                    { "a", a },
                    { "b", Clean(b) },
                    { "c", c },
                    { "d", Clean(d) },
                    { "e", e },
                    { "r", r },
                });

                output.Add(new Dictionary<string, Value> {
                    { "o", m }
                });

            }

            SingleCase((Number)1, "+", (Number)1, "+?", (Number)1);
            SingleCase((Number)2, "+", (Number)2, "*?", (Number)2);
            SingleCase((Number)3, "-?", (Number)3, "-", (Number)2);

            var ops = new[] { "+", "*", "-" };
            for (var x = 0; x < 10000; x++)
            {
                var choice = rng.NextDouble() < 0.5f;

                SingleCase(
                    (Number)rng.Next(1, 100),
                    ops[rng.Next(ops.Length)] + (choice ? "" : "?"),
                    (Number)rng.Next(1, 100),
                    ops[rng.Next(ops.Length)] + (choice ? "?" : ""),
                    (Number)rng.Next(1, 100)
                );
            }

            Generator.YololLadderGenerator(input, output);
        }
    }
}

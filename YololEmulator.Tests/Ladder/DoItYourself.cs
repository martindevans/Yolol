//using System;
//using System.Collections.Generic;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Yolol.Execution;

//namespace YololEmulator.Tests.Ladder
//{
//    [TestClass]
//    public class DoItYourself
//    {
//        [TestMethod]
//        public void GenerateDoItYourself()
//        {
//            var rng = new Random(678432);
//            var input = new List<Dictionary<string, Value>>();
//            var output = new List<Dictionary<string, Value>>();

//            void SingleCase(string expr)
//            {
//                var ms = TestExecutor.Execute("r=" + expr);
//                var v = ms.GetVariable("r").Value;

//                if (v > 10000 || v < -10000)
//                    return;

//                input.Add(new Dictionary<string, Value> {
//                    { "e", expr }
//                });

//                output.Add(new Dictionary<string, Value> {
//                    { "o", v }
//                });
//            }

//            SingleCase("1+2*3");
//            SingleCase("7/9+1");
//            SingleCase("2^4");
//            SingleCase("4%3");
//            SingleCase("1");

//            var ops = new[] { "+", "*", "-", "/", "^", "%" };
//            for (var x = 0; x < 11400; x++)
//            {
//                var expr = rng.Next(1, 100).ToString();

//                while (rng.NextDouble() > 0.35 && expr.Length < 12)
//                {
//                    expr += ops[rng.Next(ops.Length)];
//                    expr += rng.Next(1, 100).ToString();
//                }

//                SingleCase(expr);
//            }

//            Generator.YololLadderGenerator(input, output, true, Generator.ScoreMode.BasicScoring, Generator.YololChip.Professional);
//        }
//    }
//}

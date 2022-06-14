using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    [TestClass]
    public class TicTacTow
    {
        [TestMethod]
        public void GenerateTicTacToe()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            void SingleCase(int index)
            {
                var sb = new StringBuilder(9);
                for (var i = 0; i < index; i++)
                    sb.Append('_');
                sb.Append('X');
                for (var i = 0; i < 9 - index; i++)
                    sb.Append('_');

                
                for (var i = 0; i < 9; i++)
                {
                    input.Add(new()
                    {
                        { "i", new Value(sb.ToString()) },
                        { "t", (Number)i },
                    });
                    output.Add(new() { { "o", Number.One },});
                }
            }

            for (var i = 0; i < 500; i++)
            for (var j = 0; j < 9; j++)
                SingleCase(j);

            Generator.YololLadderGenerator(input, output, shuffle: false);
        }
    }
}
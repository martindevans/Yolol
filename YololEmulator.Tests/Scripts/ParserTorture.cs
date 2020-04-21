using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Grammar;

namespace YololEmulator.Tests.Scripts
{
    [TestClass]
    public class ParserTorture
    {
        [TestMethod]
        public void MethodName()
        {
            var result = Parser.ParseProgram(
                "goto TODO//comment\ngoto TODO goto TODO\n" +
                "if TODO then goto TODO else goto TODO end\n" +
                "if TODO then ww=TODO end\n" +
                "x++ ++yy --z zz--\n" +
                "c+=TODO c0-=TODO d0*=TODO d0/=TODO d0%=TODO d0^=TODO\n"
            );

            if (result.IsOk)
            {
                Console.WriteLine("```");
                Console.WriteLine(result.Ok.ToString());
                Console.WriteLine("```");
            }
            else
                Console.WriteLine(result.Err);
        }

        [TestMethod]
        public void Expression()
        {
            var result = Parser.ParseProgram(":a=:b+:c+:d goto ++:done");

            if (result.IsOk)
            {
                Console.WriteLine("```");
                Console.WriteLine(result.Ok.ToString());
                Console.WriteLine("```");
            }
            else
            {
                Console.WriteLine(result.Err);


            }
        }
    }
}

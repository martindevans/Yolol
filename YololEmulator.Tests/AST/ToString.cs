using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Grammar;

namespace YololEmulator.Tests.AST
{
    [TestClass]
    public class ToString
    {
        private static void Roundtrip(string line)
        {
            var tok = Tokenizer.TryTokenize(line);
            Assert.IsTrue(tok.HasValue);
            var par = Parser.TryParseLine(tok.Value);
            Assert.IsTrue(par.HasValue, par.FormatErrorMessageFragment());

            Assert.AreEqual(line, par.Value.ToString());
        }

        [TestMethod]
        public void Not()
        {
            Roundtrip("b=not a");
        }

        [TestMethod]
        public void And()
        {
            Roundtrip("b=a and b");
        }

        [TestMethod]
        public void Or()
        {
            Roundtrip("b=a or b");
        }

        [TestMethod]
        public void If()
        {
            Roundtrip("if a==1 then end");
        }

        [TestMethod]
        public void IfTrue()
        {
            Roundtrip("if a==1 then a=1 end");
        }

        [TestMethod]
        public void IfTrueFalse()
        {
            Roundtrip("if a==1 then a=1 else a=2 end");
        }

        [TestMethod]
        public void Goto()
        {
            Roundtrip("goto 7");
        }

        [TestMethod]
        public void Assignment()
        {
            Roundtrip("a=17");
        }

        [TestMethod]
        public void AssignmentVar()
        {
            Roundtrip("a=b");
        }

        [TestMethod]
        public void String()
        {
            Roundtrip("a=\"hi\"");
        }

        [TestMethod]
        public void Number()
        {
            Roundtrip("a=17");
        }

        [TestMethod]
        public void Negate()
        {
            Roundtrip("a=-17");
        }

        [TestMethod]
        public void PostDec()
        {
            Roundtrip("a--");
        }

        [TestMethod]
        public void PreDec()
        {
            Roundtrip("--a");
        }

        [TestMethod]
        public void PostInc()
        {
            Roundtrip("a++");
        }

        [TestMethod]
        public void PreInc()
        {
            Roundtrip("++a");
        }

        [TestMethod]
        public void Exponent()
        {
            Roundtrip("a=2^3");
        }

        [TestMethod]
        public void Modulo()
        {
            Roundtrip("a=2%3");
        }

        [TestMethod]
        public void Add()
        {
            Roundtrip("a=1+a");
        }

        [TestMethod]
        public void Subtract()
        {
            Roundtrip("a=1-a");
        }

        [TestMethod]
        public void Multiply()
        {
            Roundtrip("a=1*a");
        }

        [TestMethod]
        public void Divide()
        {
            Roundtrip("a=1/a");
        }

        [TestMethod]
        public void CompoundAdd()
        {
            Roundtrip("a+=1");
        }

        [TestMethod]
        public void CompoundSubtract()
        {
            Roundtrip("a-=1");
        }

        [TestMethod]
        public void CompoundMultiply()
        {
            Roundtrip("a*=a");
        }

        [TestMethod]
        public void CompoundDivide()
        {
            Roundtrip("a/=a");
        }

        [TestMethod]
        public void CompoundModulo()
        {
            Roundtrip("a%=3");
        }

        [TestMethod]
        public void NotEqual()
        {
            Roundtrip("a=1!=a");
        }

        [TestMethod]
        public void GreaterThan()
        {
            Roundtrip("a=1>a");
        }

        [TestMethod]
        public void GreaterThanEqualTo()
        {
            Roundtrip("a=1>=a");
        }

        [TestMethod]
        public void LessThan()
        {
            Roundtrip("a=1<a");
        }

        [TestMethod]
        public void LessThanEqualTo()
        {
            Roundtrip("a=1<=a");
        }

        [TestMethod]
        public void Brackets()
        {
            Roundtrip("a=(1<=a)");
        }

        [TestMethod]
        public void Sqrt()
        {
            Roundtrip("a=SQRT(1<=a)");
        }

        [TestMethod]
        public void Abs()
        {
            Roundtrip("a=ABS(3)");
        }
    }
}

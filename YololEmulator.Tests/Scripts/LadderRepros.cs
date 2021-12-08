using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Scripts
{
    [TestClass]
    public class LadderRepros
    {
        [TestMethod]
        public void ZijkhalBlackFriday()
        {
            var result = TestExecutor.Execute(new[] {
                ":i=\"8591433801\" a=\"*********\"i=a+9p+=a goto++k/57",
                "h=a--+8g=a--+7f=a--+6e=a--+5d=a--+4c=a--+3b=a--+2a=\"*1\"",
                "t=:i+:i q=p-0+t-a-b-c-d-e-f-g-h-i-0s=q+t l=s-s--",
                "q=q+l-a-b-c-d-e-f-g-h-i-0s=q+t m=s-s--q=q+m-a-b-c-d-e-f-g-h-i-0s=q+t+t",
                "n=s-s--q=q+n-a-b-c-d-e-f-g-h-i-0 s=q+t+t:o=l+m+n+(s-s--) goto300"
            });

            Assert.AreEqual("0000", result.GetVariable(":o").ToString());
        }

        [TestMethod]
        public void FuzzMultiplyHuge()
        {
            var ms = TestExecutor.Execute($"x=asin 1992768.34 c=1*x");

            Assert.AreEqual(-9223372036854775.808, (double)ms.GetVariable("x").Value.Number);
            Assert.AreEqual(0, (double)ms.GetVariable("c").Value.Number);
        }

        [TestMethod]
        public void FuzzExponent()
        {
            var ms = TestExecutor.Execute($"b=-9223372036854775.808 b%=-0.001");

            Assert.AreEqual(0, (double)ms.GetVariable("b").Value.Number);
        }

        [TestMethod]
        public void NyefariFactorials()
        {
            var ms = TestExecutor.Execute($"a=13! b=13*(12!)");

            Assert.AreEqual(6227020800, (double)ms.GetVariable("a").Value.Number);
            Assert.AreEqual(6227020800, (double)ms.GetVariable("b").Value.Number);
        }

        [TestMethod]
        public void ZijkhalExponents()
        {
            var a = 0.315;
            var b = Math.Pow(a, 1);
            Console.WriteLine(a);
            Console.WriteLine(b);
            Assert.AreEqual(a, b);

            var ms = TestExecutor.Execute($"a=0.315 b=a^1");

            Assert.AreEqual(0.315, (double)(Number)ms.GetVariable("b").Value.Number);
        }

        [TestMethod]
        public void MartinChallenge1()
        {
            TestExecutor.Parse(":d=:b+:c+:a:done=1goto1");
        }

        [TestMethod]
        public void PyryChallenge1()
        {
            TestExecutor.Parse(":d=:b+:c+:a goto++:done");
        }

        [TestMethod]
        public void AraliciaPreChallenge2()
        {
            TestExecutor.Parse(" a = 1");
        }

        [TestMethod]
        public void AraliciaPostChallenge2()
        {
            TestExecutor.Parse("a = 1 ");
        }

        [TestMethod]
        public void ZijkhalStrings()
        {
            //for (var i = 0; i < 4010; i++)
            var i = 3600;
            {
                try
                {
                    var ms = TestExecutor.Execute($":a={i} a=\" seconds1 \"b=\" minutes1\"e=\" hours1\"f=\" days1\"g=\" years1\"q=\",1\"",
                        "t=\" \"n=\"s1\"k=0+e-n+t l=\"0 day \"p=\" and1\"r=0+g-n+t",
                        "y=:a s=y%60y-=s y/=60m=y%60y-=m y/=60h=y%24y-=h y/=24d=y%365y-=d",
                        "o=a-(s>1)-n c=s>0v=m>0o=b-(m>1)-n+p-c*v-p+t+s+o y/=365c+=v",
                        "v=h>0o=e-(h>1)-n+q-v*(c>1)-q+p-c*v-p+t+m+o c+=v v=d>0u=f-(d>1)-n",
                        "u+=q-v*(c>1)-q+p-c*v-p+t+h+o c+=v v=y>0o=g-(y>1)-n+q-(y>0)*(c>1)-q",
                        ":o=y+o+(p-c*(y>0)-p)+t+d+u-r-l-k-(0+b-n+t)-(0+a-n)-t:done=1goto50"
                    );
                }
                catch
                {
                    Console.WriteLine("Index: " + i);
                    throw;
                }
            }
        }

        //[TestMethod]
        //public void EvilStrings()
        //{
        //    var ms = TestExecutor.Execute(
        //        "a=\"abcdef\"",
        //        "a -= \"a\"",
        //        "b = a---a"
        //    );

        //    Assert.AreEqual("bcde", ms.GetVariable("a").Value.ToString());
        //    Assert.AreEqual("f", ms.GetVariable("b").Value.ToString());
        //}

        [TestMethod]
        public void EvilStrings2()
        {
            var ms = TestExecutor.Execute(
                "a=\"abcxabc\"",
                "b=a b-- b-- b-- b--",
                "c=a-b"
            );

            Assert.AreEqual("abcxabc", ms.GetVariable("a").Value.ToString());
            Assert.AreEqual("abc", ms.GetVariable("b").Value.ToString());
            Assert.AreEqual("abcx", ms.GetVariable("c").Value.ToString());
        }
    }
}

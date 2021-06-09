using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Scripts
{
    [TestClass]
    public class LadderRepros
    {
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

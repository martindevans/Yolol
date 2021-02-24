using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YololEmulator.Tests.Scripts
{
    [TestClass]
    public class LadderRepros
    {
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
        public void GrahamOrder()
        {
            var result = TestExecutor.Execute("a=10000*(12.345/10000) b=10000*12.345/10000");

            Assert.AreEqual(10m, result.GetVariable("a").Value);
            Assert.AreEqual(10m, result.GetVariable("b").Value);
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

        [TestMethod]
        public void EvilStrings()
        {
            var ms = TestExecutor.Execute(
                "a=\"abcdef\"",
                "a -= \"a\"",
                "b = a---a"
            );

            Assert.AreEqual("bcde", ms.GetVariable("a").Value.ToString());
            Assert.AreEqual("f", ms.GetVariable("b").Value.ToString());
        }

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

using System;
using System.Linq;
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
    }
}

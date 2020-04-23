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
    }
}

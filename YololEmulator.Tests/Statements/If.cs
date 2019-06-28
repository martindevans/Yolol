using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YololEmulator.Tests.Statements
{
    [TestClass]
    public class If
    {
        [TestMethod]
        public void IfNone()
        {
            var result = TestExecutor.Execute("a = 1", "b = 2 if a == 1 then else end");

            var a = result.GetVariable("a");

            Assert.AreEqual(1, a.Value.Number);
        }

        [TestMethod]
        public void IfTrue()
        {
            var result = TestExecutor.Execute("a = 1", "if a == 1 then a = 2 end");

            var a = result.GetVariable("a");

            Assert.AreEqual(2, a.Value.Number);
        }

        [TestMethod]
        public void IfFalse()
        {
            var result = TestExecutor.Execute("a = 1", "if a == 2 then a = 2 end");

            var a = result.GetVariable("a");

            Assert.AreEqual(1, a.Value.Number);
        }

        [TestMethod]
        public void IfElseTrue()
        {
            var result = TestExecutor.Execute("a = 1", "if a == 1 then a = 2 else a = 3 end");

            var a = result.GetVariable("a");

            Assert.AreEqual(2, a.Value.Number);
        }

        [TestMethod]
        public void IfElseFalse()
        {
            var result = TestExecutor.Execute("a = 1", "if a == 2 then a = 2 else a = 3 end");

            var a = result.GetVariable("a");

            Assert.AreEqual(3, a.Value.Number);
        }

        [TestMethod]
        public void IfSkipGoto()
        {
            var result = TestExecutor.Execute(
                "a = 1",
                "if a == 1 then goto 4 end",
                "a = 2"
            );

            var a = result.GetVariable("a");
            Assert.AreEqual(1, a.Value.Number);
        }
    }
}

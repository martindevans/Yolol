using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YololEmulator.Tests.Scripts
{
    [TestClass]
    public class PointlessNumbers
    {
        [TestMethod]
        public void ChaosAlphaSimplified()
        {
            var lines = new[] {
                "a++ b-- --c --c ++c"
            };

            var result = TestExecutor.Execute(lines);

            Assert.AreEqual(1, result.Get("a").Value.Number);
            Assert.AreEqual(-1, result.Get("b").Value.Number);
            Assert.AreEqual(-1, result.Get("c").Value.Number);
        }
    }
}

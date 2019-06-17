using Microsoft.VisualStudio.TestTools.UnitTesting;
using YololEmulator.Execution;

namespace YololEmulator.Tests
{
    [TestClass]
    public class VariableTests
    {
        [TestMethod]
        public void ToStringEqualsValue()
        {
            var v = new Variable {
                Value = new Value(3)
            };
            Assert.AreEqual(v.ToString(), v.Value.ToString());
        }
    }
}

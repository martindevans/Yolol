using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YololEmulator.Execution;

namespace YololEmulator.Tests
{
    [TestClass]
    public class MachineStateTests
    {
        [TestMethod]
        public void EnumerateEmpty()
        {
            var s = new MachineState(new ConstantNetwork()).ToArray();
            Assert.AreEqual(0, s.Length);
        }

        [TestMethod]
        public void Enumerate()
        {
            var s = new MachineState(new ConstantNetwork());

            s.Get("name");

            var arr = s.ToArray();
            Assert.AreEqual(1, arr.Length);
        }

        [TestMethod]
        public void ExternalVariable()
        {
            var n = new ConstantNetwork();
            n.Get("name").Value = new Value(13);
            var s = new MachineState(n);

            Assert.AreEqual(13, s.Get(":name").Value.Number);
        }
    }
}

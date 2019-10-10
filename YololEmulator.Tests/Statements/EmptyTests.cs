using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;
using Yolol.Grammar.AST.Statements;

namespace YololEmulator.Tests.Statements
{
    [TestClass]
    public class EmptyTests
    {
        [TestMethod]
        public void Evaluate()
        {
            var s = new MachineState(new NullDeviceNetwork());
            new EmptyStatement().Evaluate(s);

            Assert.AreEqual(0, s.Count());
        }

        [TestMethod]
        public new void ToString()
        {
            Assert.AreEqual("", new EmptyStatement().ToString());
        }

        [TestMethod]
        public void DoesNotError()
        {
            Assert.IsFalse(new EmptyStatement().CanRuntimeError);
        }
    }
}

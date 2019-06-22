using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Expressions
{
    [TestClass]
    public class Application
    {
        [TestMethod]
        public void CallUnknown()
        {
            Assert.ThrowsException<ExecutionException>(() => {
                TestExecutor.Execute("a = unknown(1)");
            });
        }
    }
}

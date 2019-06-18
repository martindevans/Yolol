using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Expressions.Str
{
    [TestClass]
    public class Modulo
    {
        [TestMethod]
        public void ConstantConstant()
        {
            Assert.ThrowsException<ExecutionException>(() => {
                TestExecutor.Execute("a = \"a\" % \"b\"");
            });
        }

        [TestMethod]
        public void VariableConstant()
        {
            Assert.ThrowsException<ExecutionException>(() => {
                TestExecutor.Execute("a = \"a\"", "b = a % \"b\"");
            });
        }

        [TestMethod]
        public void ConstantVariable()
        {
            Assert.ThrowsException<ExecutionException>(() => {
                TestExecutor.Execute("a = \"a\"", "b = \"b\" % a");
            });
        }
    }
}

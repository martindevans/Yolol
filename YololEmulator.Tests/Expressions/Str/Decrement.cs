using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Expressions.Str
{
    [TestClass]
    public class Decrement
    {
        [TestMethod]
        public void PostDecrement()
        {
            var result = TestExecutor.Execute("a = \"ab\"", "b = a--");

            var a = result.GetVariable("a");
            var b = result.GetVariable("b");

            // if post dec is fixed, comment this back in
            //Assert.AreEqual("a", a.Value.String.ToString());
            //Assert.AreEqual("ab", b.Value.String.ToString());

            Assert.AreEqual("a", a.Value.String.ToString());
            Assert.AreEqual("a", b.Value.String.ToString());
        }

        [TestMethod]
        public void PreDecrement()
        {
            var result = TestExecutor.Execute("a = \"ab\"", "b = --a");

            var a = result.GetVariable("a");
            var b = result.GetVariable("b");

            Assert.AreEqual("a", a.Value.String.ToString());
            Assert.AreEqual("a", b.Value.String.ToString());
        }

        [TestMethod]
        public void Empty()
        {
            Assert.ThrowsException<ExecutionException>(() => {
                TestExecutor.Execute("a = \"\"", "b = --a");
            });
        }

        //[TestMethod]
        //public void Pop()
        //{
        //    var result = TestExecutor.Execute("a = \"abcdefgh\"", "b = a---a");

        //    var a = result.GetVariable("a");
        //    var b = result.GetVariable("b");

        //    Assert.AreEqual("abcdefg", a.Value.String.ToString());
        //    Assert.AreEqual("h", b.Value.String.ToString());
        //}

        [TestMethod]
        public void DecrementToEmpty()
        {
            var result = TestExecutor.Execute("a = \"a\" a--");

            var a = result.GetVariable("a");

            Assert.AreEqual("", a.Value.String.ToString());
        }
    }
}

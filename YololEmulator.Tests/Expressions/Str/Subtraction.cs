using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YololEmulator.Tests.Expressions.Str
{
    [TestClass]
    public class Subtraction
    {
        [TestMethod]
        public void ConstantConstant()
        {
            var result = TestExecutor.Execute("a = \"abc\" - \"b\"");

            var a = result.GetVariable("a");

            Assert.AreEqual("ac", a.Value.String.ToString());
        }

        [TestMethod]
        public void ConstantConstant2()
        {
            var result = TestExecutor.Execute("a = \"doggo\" - \"o\"");

            var a = result.GetVariable("a");

            Assert.AreEqual("dogg", a.Value.String.ToString());
        }

        [TestMethod]
        public void ConstantConstant_NothingRemoved()
        {
            var result = TestExecutor.Execute("a = \"abc\" - \"d\"");

            var a = result.GetVariable("a");

            Assert.AreEqual("abc", a.Value.String.ToString());
        }

        [TestMethod]
        public void VariableConstant()
        {
            var result = TestExecutor.Execute("a = \"abc\"", "b = a - \"b\"");

            var a = result.GetVariable("a");
            var b = result.GetVariable("b");

            Assert.AreEqual("abc", a.Value.String.ToString());
            Assert.AreEqual("ac", b.Value.String.ToString());
        }

        [TestMethod]
        public void ConstantVariable()
        {
            var result = TestExecutor.Execute("a = \"b\"", "b = \"abc\" - a");

            var a = result.GetVariable("a");
            var b = result.GetVariable("b");

            Assert.AreEqual("b", a.Value.String.ToString());
            Assert.AreEqual("ac", b.Value.String.ToString());
        }

        [TestMethod]
        public void RemoveLastCharacter()
        {
            var result = TestExecutor.Execute("a = \"abc\" - \"c\"");

            var a = result.GetVariable("a");

            Assert.AreEqual("ab", a.Value.String.ToString());
        }

        [TestMethod]
        public void RemoveLastCharacters()
        {
            var result = TestExecutor.Execute("a = \"abcsdfsdf\" - \"sdfsdf\"");

            var a = result.GetVariable("a");

            Assert.AreEqual("abc", a.Value.String.ToString());
        }

        [TestMethod]
        public void RemoveFirstCharacter()
        {
            var result = TestExecutor.Execute("a = \"abc\" - \"a\"");

            var a = result.GetVariable("a");

            Assert.AreEqual("bc", a.Value.String.ToString());
        }

        [TestMethod]
        public void RemoveMidCharacter()
        {
            var result = TestExecutor.Execute("a = \"abc\" - \"b\"");

            var a = result.GetVariable("a");

            Assert.AreEqual("ac", a.Value.String.ToString());
        }
    }
}

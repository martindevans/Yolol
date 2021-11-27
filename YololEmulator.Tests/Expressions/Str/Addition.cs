using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Text;
using Yolol.Execution;

namespace YololEmulator.Tests.Expressions.Str
{
    [TestClass]
    public class Addition
    {
        [TestMethod]
        public void SaturateOnes()
        {
            var result = TestExecutor.Execute(
                "a=\"a\" c=500",
                "a+=1 c-- goto2+(c==0)"
            );

            var a = result.GetVariable("a");

            var expected = new StringBuilder("a");
            for (int i = 0; i < 500; i++)
                expected.Append('1');

            Assert.AreEqual(new YString(expected.ToString()), a.Value.String);
        }

        [TestMethod]
        public void ConstantConstant()
        {
            var result = TestExecutor.Execute("a = \"ad\" + \"bc\"");

            var a = result.GetVariable("a");

            Assert.AreEqual(new YString("adbc"), a.Value.String);
        }

        [TestMethod]
        public void VariableConstant()
        {
            var result = TestExecutor.Execute("a = \"a\"", "b = a + \"b\"");

            var a = result.GetVariable("a");
            var b = result.GetVariable("b");

            Assert.AreEqual(new YString("a"), a.Value.String);
            Assert.AreEqual(new YString("ab"), b.Value.String);
        }

        [TestMethod]
        public void ConstantVariable()
        {
            var result = TestExecutor.Execute("a = \"a\"", "b = \"b\" + a");

            var a = result.GetVariable("a");
            var b = result.GetVariable("b");

            Assert.AreEqual(new YString("a"), a.Value.String);
            Assert.AreEqual(new YString("ba"), b.Value.String);
        }
    }
}

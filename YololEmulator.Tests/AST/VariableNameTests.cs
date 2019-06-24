using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Grammar;

namespace YololEmulator.Tests.AST
{
    [TestClass]
    public class VariableNameTests
    {
        [TestMethod]
        public void Equal()
        {
            var a = new VariableName("aaa");
            var b = new VariableName("aaa");

            Assert.IsTrue(a == b);
            // ReSharper disable once EqualExpressionComparison
            Assert.IsTrue(a == a);

            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(a.Equals(a));

            Assert.IsTrue(a.Equals((object)b));
            Assert.IsTrue(a.Equals((object)a));

            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [TestMethod]
        public void NotEqual()
        {
            var a = new VariableName("aaa");
            var b = new VariableName("bbb");

            Assert.IsTrue(a != b);

            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a.Equals(null));

            Assert.IsFalse(a.Equals((object)b));
            Assert.IsFalse(a.Equals((object)null));
            // ReSharper disable once SuspiciousTypeConversion.Global
            Assert.IsFalse(a.Equals(1));

            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }
    }
}

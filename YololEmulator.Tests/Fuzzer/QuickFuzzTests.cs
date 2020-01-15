using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Analysis.Fuzzer;

namespace YololEmulator.Tests.Fuzzer
{
    [TestClass]
    public class QuickFuzzTests
    {
        [TestMethod]
        public async Task FuzzerRunsProgram()
        {
            var p = TestExecutor.Parse("a = :a b = :b :c = a + b");
            var f = new QuickFuzz();

            var results = await f.Fuzz(p);

            foreach (var result in results)
            {
                var a = result.Gets[0];
                var b = result.Gets[1];
                var c = result.Sets[0];

                Assert.AreEqual(a.Item3 + b.Item3, c.Item3);
            }
        }

        [TestMethod]
        public async Task TwoRunsAreIdentical()
        {
            var p = TestExecutor.Parse("a = :a b = :b :c = a + b");
            var f = new QuickFuzz();

            var r1 = await f.Fuzz(p);
            var r2 = await f.Fuzz(p);

            Assert.IsTrue(r1.Equals(r2));
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YololEmulator.Tests.Scripts.Acid
{
    [TestClass]
    public class Numbers
    {
        [TestMethod]
        public void SqrtA()
        {
            var ms = TestExecutor.Execute(
                "x=sqrt 24",
                "y=sqrt 23.999"
            );

            Assert.AreEqual("4.898", ms.GetVariable("y").Value.Number.ToString());
            Assert.AreEqual("4.899", ms.GetVariable("x").Value.Number.ToString());
        }

        [TestMethod]
        public void SqrtB()
        {
            var ms = TestExecutor.Execute(
                "x=sqrt 2"
            );

            Assert.AreEqual("1.414", ms.GetVariable("x").Value.Number.ToString());
        }

        [TestMethod]
        public void SqrtC()
        {
            var ms = TestExecutor.Execute(
                "x=sqrt 7"
            );

            Assert.AreEqual("2.645", ms.GetVariable("x").Value.Number.ToString());
        }

        [TestMethod]
        public void SqrtD()
        {
            var ms = TestExecutor.Execute(
                "x=sqrt 32199"
            );

            Assert.AreEqual("179.44", ms.GetVariable("x").Value.Number.ToString());
        }

        [TestMethod]
        public void SqrtE()
        {
            var ms = TestExecutor.Execute(
                "x=sqrt 1000001"
            );

            Assert.AreEqual("1000", ms.GetVariable("x").Value.Number.ToString());
        }

        [TestMethod]
        public void SqrtF()
        {
            var ms = TestExecutor.Execute(
                "x=sqrt 1000002"
            );

            Assert.AreEqual("1000", ms.GetVariable("x").Value.Number.ToString());
        }

        [TestMethod]
        public void SqrtG()
        {
            var ms = TestExecutor.Execute(
                "x=sqrt 9223372036854775.807"
            );

            Assert.AreEqual("-9223372036854775.808", ms.GetVariable("x").Value.Number.ToString());
        }

        [TestMethod]
        public void SqrtH()
        {
            var ms = TestExecutor.Execute(
                "x=sqrt -3"
            );

            Assert.AreEqual("-9223372036854775.808", ms.GetVariable("x").Value.Number.ToString());
        }

        [TestMethod]
        public void SqrtI()
        {
            var ms = TestExecutor.Execute(
                "x=sqrt 9223372036854775"
            );

            Assert.AreEqual("-9223372036854775.808", ms.GetVariable("x").Value.Number.ToString());
        }

        [TestMethod]
        public void SqrtJ()
        {
            var ms = TestExecutor.Execute(
                "x=sqrt 9223372036854774.999"
            );

            Assert.AreEqual("96038388.349", ms.GetVariable("x").Value.Number.ToString());
        }

        //[TestMethod]
        //public void Sqrt()
        //{
        //    var ms = TestExecutor.Execute(
        //        "n=1 x=sqrt 24 y=4.899 if x!=y then goto19 end n++ ",
        //        "x=(sqrt 2) y=1.414 if x!=y then goto19 end n++ ",
        //        "x=(sqrt 7) y=2.645 if x!=y then goto19 end n++ ",
        //        "x=(sqrt 32199) y=179.440 if x!=y then goto19 end n++ ",
        //        "x=(sqrt 1000001) y=1000 if x!=y then goto19 end n++ ",
        //        "x=(sqrt 1000002) y=1000 if x!=y then goto19 end n++ ",
        //        "x=sqrt 9223372036854775.807 y=-9223372036854775.808 n++ goto19/(x!=y)",
        //        "x=(sqrt -3) y=-9223372036854775.808 if x!=y then goto19 end n++ ",
        //        "x=sqrt 9223372036854775 y=-9223372036854775.808 n++ goto19/(x!=y) ",
        //        "x=sqrt 9223372036854774.999 y=96038388.349 n++ goto19/(x!=y)",
        //        "",
        //        "",
        //        "",
        //        "",
        //        "",
        //        "",
        //        "if n != 11 then OUTPUT=\"Skipped: \"+(11-n)+\" tests\" goto 20 end",
        //        "OUTPUT=\"ok\" goto20",
        //        "OUTPUT=\"Failed test #\"+n+\" got: \"+x+\" but wanted: \"+y",
        //        ""
        //    );

        //    Assert.AreEqual("ok", ms.GetVariable("output").Value.String.ToString());
        //}
    }
}

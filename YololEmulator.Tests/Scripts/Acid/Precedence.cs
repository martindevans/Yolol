using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YololEmulator.Tests.Scripts.Acid
{
    [TestClass]
    public class Precedence
    {
        [TestMethod]
        public void Precedence1()
        {
            var ms = TestExecutor.Execute(
                "num=1 x=(0 and 0 or 1 ) y=0 if x!=y then goto19 end num++",
                "x=((0 and 0) or 1 ) y= 1 if x!=y then goto19 end num++ ",
                "x=(0 and (0 or 1) ) y= 0 if x!=y then goto19 end num++ ",
                "x=(5+5-6 ) y= 4 if x!=y then goto19 end num++ ",
                "x=(-6+5+5 ) y= 4 if x!=y then goto19 end num++ ",
                "x=(5-6+5 ) y= 4 if x!=y then goto19 end num++ ",
                "x=(2*5/4 ) y=2.5 if x!=y then goto19 end num++ ",
                "x=(10/(2*4) ) y= 1.25 if x!=y then goto19 end num++ ",
                "x=(10/2*4 ) y= 20 if x!=y then goto19 end num++ ",
                "x=(2+2*2 ) y= 6 if x!=y then goto19 end num++ ",
                "a=1 x=(5*a++ ) y= 10 if x!=y then goto19 end num++ ",
                "a=2 x=(5*a-- ) y= 5 if x!=y then goto19 end num++ ",
                "a=2 x=(-a++ ) y= -3 if x!=y then goto19 end num++ ",
                "a=2 x=(-a! ) y= -2  if x!=y then goto19 end num++ ",
                "a=2 x=(-(a!) ) y= -2 if x!=y then goto19 end num++ ",
                "",
                "if num != 16 then OUTPUT=\"Skipped: \"+(16-num)+\" tests\" goto 20 end",
                "OUTPUT=\"ok\" goto20",
                "OUTPUT=\"Failed test #\"+num+\" got: \"+x+\" but wanted: \"+y",
                ""
            );

            Assert.AreEqual("ok", ms.GetVariable("output").Value.String.ToString());
        }

        [TestMethod]
        public void Precedence2()
        {
            var ms = TestExecutor.Execute(
                "num=1 x=(sqrt 3! ) y=2.449 if x!=y then goto19 end num++ ",
                "x=(sqrt (3!) ) y=2.449 if x!=y then goto19 end num++ ",
                "x=((sqrt 9) ) y=3 if x!=y then goto19 end num++ ",
                "x=((abs 3) ) y=3 if x!=y then goto19 end num++ ",
                "a=2+2 x=(a! ) y=24  if x!=y then goto19 end num++ ",
                "x=(2+3! ) y=8 if x!=y then goto19 end num++ ",
                "x=(2*3! ) y=12 if x!=y then goto19 end num++ ",
                "a=-3 x=(a! ) y=-9223372036854775.808 if x!=y then goto19 end num++ ",
                "a=-3 x=(abs a! ) y=-9223372036854775.808 if x!=y then goto19 end num++ ",
                "a=-3 x=(abs (a!) ) if x!=y then goto19 end num++ ",
                "",
                "",
                "",
                "",
                "",
                "",
                "if num != 11 then OUTPUT=\"Skipped: \"+(11-num)+\" tests\" goto 20 end",
                "OUTPUT=\"ok\" goto20",
                "OUTPUT=\"Failed test #\"+num+\" got: \"+x+\" but wanted: \"+y",
                ""
            );

            Assert.AreEqual("ok", ms.GetVariable("output").Value.String.ToString());
        }

        [TestMethod]
        public void Precedence3()
        {
            var ms = TestExecutor.Execute(
                "num=1 x=(2*2^2 ) y= 8 if x!=y then goto19 end num++ ",
                "x=(2+2^2 ) y= 6 if x!=y then goto19 end num++ ",
                "x=(-2^2 ) y= 4 if x!=y then goto19 end num++ ",
                "x=(-(2^2) ) y= -4 if x!=y then goto19 end num++ ",
                "x=(sqrt 3+6 ) y= 7.732 if x!=y then goto19 end num++ ",
                "x=(sqrt (3+6) ) y= 3 if x!=y then goto19 end num++ ", 
                "x=(sqrt 3*3 ) y= 5.196 if x!=y then goto19 end num++ ",
                "x=(abs -5+5 ) y= 10 if x!=y then goto19 end num++ ",
                "x=(abs (-5+5) ) y= 0 if x!=y then goto19 end num++ ",
                "x=(sin (1^2) ) y= 0.017 if x!=y then goto19 end num++ ",
                "x=((sin 1)^2 ) y= 0 if x!=y then goto19 end num++ ", 
                "x=(sin 1^2 ) y= 0 if x!=y then goto19 end num++ ",
                "x=(2+2>1+1 ) y= 4 if x!=y then goto19 end num++ ",
                "x=(2+2>=1+1 ) y= 4 if x!=y then goto19 end num++ ",
                "x=(2*2>1*1 ) y= 1 if x!=y then goto19 end num++ ",
                "x=(2*2>=1*1 ) y= 1 if x!=y then goto19 end num++ ",
                "if num != 17 then OUTPUT=\"Skipped: \"+(17-num)+\" tests\" goto 20 end",
                "OUTPUT=\"ok\" goto20",
                "OUTPUT=\"Failed test #\"+num+\" got: \"+x+\" but wanted: \"+y",
                ""
            );

            Assert.AreEqual("ok", ms.GetVariable("output").Value.String.ToString());
        }

        [TestMethod]
        public void Precedence4()
        {
            var ms = TestExecutor.Execute(
                "num=1 x=(2*(2>1)*1 ) y= 2 if x!=y then goto19 end num++ ",
                "x=(2^2>1^1 ) y= 1 if x!=y then goto19 end num++ ",
                "x=(2+1==1+2 ) y= 5 if x!=y then goto19 end num++ ",
                "x=(2*1==1*2 ) y= 1 if x!=y then goto19 end num++ ",
                "x=(0==1>1==1 ) y= 0 if x!=y then goto19 end num++ ",
                "x=((0==1)>(1==1) ) y= 0 if x!=y then goto19 end num++ ",
                "x=(0==(1>1)==1 ) y= 1 if x!=y then goto19 end num++ ",
                "x=((((0==1)>1)==1) ) y= 0 if x!=y then goto19 end num++ ",
                "x=(0>1==0 ) y= 1 if x!=y then goto19 end num++ ",
                "x=((0>1)==0 ) y= 1 if x!=y then goto19 end num++ ",
                "x=(0>(1==0) ) y= 0 if x!=y then goto19 end num++ ",
                "x=(0==(0 or 1)==1 ) y= 0 if x!=y then goto19 end num++ ",
                "x=(0==0 or 1==1 ) y= 1 if x!=y then goto19 end num++ ",
                "x=(1 or 0 == 0 ) y= 1 if x!=y then goto19 end num++ ",
                "x=((1 or 0) == 0 ) y= 0 if x!=y then goto19 end num++ ",
                "x=(1 or (0 == 0) ) y= 1 if x!=y then goto19 end num++ ",
                "if num != 17 then OUTPUT=\"Skipped: \"+(17-num)+\" tests\" goto 20 end",
                "OUTPUT=\"ok\" goto20",
                "OUTPUT=\"Failed test #\"+num+\" got: \"+x+\" but wanted: \"+y",
                ""
            );

            Assert.AreEqual("ok", ms.GetVariable("output").Value.String.ToString());
        }

        [TestMethod]
        public void Precedence5()
        {
            var ms = TestExecutor.Execute(
                "num=1 x=(not 1+1 ) y=0 if x!=y then goto19 end num++ ",
                "x=(not 0+1 ) y=0 if x!=y then goto19 end num++ ",
                "x=(not 0+0 ) y=1 if x!=y then goto19 end num++ ",
                "x=(not (1+1) ) y=0 if x!=y then goto19 end num++ ",
                "x=((not 1)+1 ) y=1 if x!=y then goto19 end num++ ",
                "x=((not 0)+1 ) y=2 if x!=y then goto19 end num++ ",
                "x=(not (1 and 1) ) y=0 if x!=y then goto19 end num++ ",
                "x=(not (1 and 0) ) y=1 if x!=y then goto19 end num++ ",
                "x=((not 1) and 1 )  y=0 if x!=y then goto19 end num++ ",
                "x=((not 0) and 1 ) y=1 if x!=y then goto19 end num++ ",
                "x=((not 0) and 0 ) y=0 if x!=y then goto19 end num++ ",
                "x=(1 and not 0 and 1) y=1 if x!=y then goto19 end num++", 
                "x=(1 and not 1 and 1) y=0 if x!=y then goto19 end num++ ",
                "x=(1 and not 0 and 0) y=0 if x!=y then goto19 end num++ ",
                "x=(1 and not (0 and 0)) y=1 if x!=y then goto19 end num++ ",
                "x=(1 and not 0) y=1 if x!=y then goto19 end num++ ",
                "if num != 17 then OUTPUT=\"Skipped: \"+(17-num)+\" tests\" goto 20 end",
                "OUTPUT=\"ok\" goto20",
                "OUTPUT=\"Failed test #\"+num+\" got: \"+x+\" but wanted: \"+y",
                ""
            );

            Assert.AreEqual("ok", ms.GetVariable("output").Value.String.ToString());
        }

        [TestMethod]
        public void Precedence6()
        {
            var ms = TestExecutor.Execute(
                "num=1 x=not(not 0) y=0 ifx!=y thengoto19end num++ ",
                "x=not(not 1) y=1 ifx!=y thengoto19end num++ ",
                "x=(not 0) and not 0 y=1 ifx!=y thengoto19end num++ ",
                "x=(not 0) and not 1 y=0 ifx!=y thengoto19end num++ ",
                "x=1+(not 1) y=1 ifx!=y thengoto19end num++ ",
                "x=1+(not 0) y=2 ifx!=y thengoto19end num++ ",
                "x=not 1+1 y=0 ifx!=y thengoto19end num++ ",
                "x=not 0+1 y=0 ifx!=y thengoto19end num++ ",
                "x=not 0+0 y=1 ifx!=y thengoto19end num++ ",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "ifnum!=10then OUTPUT=\"Skipped: \"+(10-num)+\" tests\" goto20end",
                "OUTPUT=\"ok\" goto20",
                "OUTPUT=\"Failed test #\"+num+\" got: \"+x+\" but wanted: \"+y",
                ""
            );

            Assert.AreEqual("ok", ms.GetVariable("output").Value.String.ToString());
        }
    }
}

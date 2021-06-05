using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Scripts.Acid
{
    [TestClass]
    public class StringLogic
    {
        [TestMethod]
        public void AcidStringLogic()
        {
            var ms = TestExecutor.Execute($"num=1 if \"\" then goto 19 end num++",
                "if \"abc\" then goto 19 end num++", 
                "if \"1\" then goto 19 end num++", 
                "if \"0\" then goto 19 end num++", 
                "if not \"\" then goto 19 end num++", 
                "if not \"1\" then goto 19 end num++", 
                "if not \"0\" then goto 19 end num++", 
                "if 1 and \"\" then goto 19 end num++", 
                "if 1 and \"1\" then goto 19 end num++", 
                "if 1 and \"0\" then goto 19 end num++", 
                "if not (1 or \"\") then goto 19 end num++", 
                "if not (1 or \"1\") then goto 19 end num++", 
                "if not (1 or \"0\") then goto 19 end num++", 
                "if 0 or \"\" then goto 19 end num++", 
                "if 0 or \"1\" then goto 19 end num++", 
                "if 0 or \"0\" then goto 19 end num++", 
                "if num != 17 then OUTPUT=\"Skipped: \"+(17-num)+\" tests\" goto 20 end", 
                "OUTPUT=\"ok\" goto20", 
                "OUTPUT=\"Failed test #\"+num", 
                ""
            );

            Assert.AreEqual("ok", ms.GetVariable("output").Value.String.ToString());
        }
    }
}

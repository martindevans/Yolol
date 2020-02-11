using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Z3;

namespace YololEmulator.Tests.SAT
{
    [TestClass]
    public class Playground
    {
        [TestMethod]
        public void Z3_Int2Str()
        {
            //ctx.IntToString

            using (var ctx = new Context())
            using (var solver = ctx.MkSolver())
            {

                // sqrt
                var big = (IntExpr)ctx.MkConst("big", ctx.IntSort);
                var lil = (IntExpr)ctx.MkConst("lil", ctx.IntSort);
                solver.Assert(lil * lil / 1000 <= big);
                solver.Assert((lil + 1) * (lil + 1) / 1000 >= big);
                solver.Assert(ctx.MkEq(big, ctx.MkInt(75321)));
                solver.Assert(lil >= ctx.MkInt(0));


                //var a = (IntExpr)ctx.MkConst("a", ctx.IntSort);
                //solver.Assert(ctx.MkEq(ctx.MkInt(9110), a));

                //// convert int to string
                //var x = (SeqExpr)ctx.MkConst("x", ctx.StringSort);
                //solver.Assert(
                //    ctx.MkEq(x,
                //        ctx.IntToString(
                //            ctx.MkITE(
                //                ctx.MkLt(a, ctx.MkInt(0)),
                //                ctx.MkMul(ctx.MkInt(-1), a),
                //                a
                //            )
                //        )
                //    )
                //);

                //var before = (IntExpr)ctx.MkConst("before", ctx.IntSort);
                //solver.Assert(ctx.MkEq(before, ctx.MkDiv(a, ctx.MkInt(1000))));

                //var after = (IntExpr)ctx.MkConst("after", ctx.IntSort);
                //solver.Assert(ctx.MkEq(after, a - before * 1000));

                //// store length
                //var y = (IntExpr)ctx.MkConst("y", ctx.IntSort);
                //solver.Assert(ctx.MkEq(y, ctx.MkLength(x)));

                //var z1 = (SeqExpr)ctx.MkConst("z1", ctx.StringSort);
                //solver.Assert(ctx.MkIff(ctx.MkLt(a, ctx.MkInt(10)), ctx.MkEq(z1, ctx.MkConcat(ctx.MkString("0.00"), x))));
                //solver.Assert(ctx.MkIff(ctx.MkAnd(ctx.MkLt(a, ctx.MkInt(100)), ctx.MkGt(a, ctx.MkInt(9))), ctx.MkEq(z1, ctx.MkConcat(ctx.MkString("0.0"), x))));
                //solver.Assert(ctx.MkIff(ctx.MkAnd(ctx.MkLt(a, ctx.MkInt(1000)), ctx.MkGt(a, ctx.MkInt(99))), ctx.MkEq(z1, ctx.MkConcat(ctx.MkString("0."), x))));
                //solver.Assert(ctx.MkIff(ctx.MkGt(a, ctx.MkInt(999)), ctx.MkEq(z1, ctx.MkConcat(ctx.MkExtract(x, ctx.MkInt(0), (IntExpr)ctx.MkSub(y, ctx.MkInt(3))), ctx.MkString("."), ctx.MkExtract(x, (IntExpr)ctx.MkSub(y, ctx.MkInt(3)), ctx.MkInt(3))))));


                //// Handle all the cases which need extra zeroes prepended, otherwise just stick a "." in the right place. After that, add a negative symbol if necessary
                //var handleN = ctx.MkConcat(ctx.MkExtract(x, ctx.MkInt(0), (IntExpr)ctx.MkSub(y, ctx.MkInt(3))), ctx.MkString("."), ctx.MkExtract(x, (IntExpr)ctx.MkSub(y, ctx.MkInt(3)), ctx.MkInt(3)));
                //var handle3 = (SeqExpr)ctx.MkITE(ctx.MkEq(y, ctx.MkInt(3)), ctx.MkConcat(ctx.MkString("0."), x), handleN);
                //var handle2 = (SeqExpr)ctx.MkITE(ctx.MkEq(y, ctx.MkInt(2)), ctx.MkConcat(ctx.MkString("0.0"), x), handle3);
                //var handle1 = (SeqExpr)ctx.MkITE(ctx.MkEq(y, ctx.MkInt(1)), ctx.MkConcat(ctx.MkString("0.00"), x), handle2);
                //var handle0 = (SeqExpr)ctx.MkITE(ctx.MkEq(y, ctx.MkInt(0)), ctx.MkString("0"), handle1);
                //var handleNeg = (SeqExpr)ctx.MkITE(ctx.MkLt(a, ctx.MkInt(0)), ctx.MkConcat(ctx.MkString("-"), handle0), handle0);
                //var handleEnd = (SeqExpr)ctx.MkITE(ctx.MkSuffixOf(ctx.MkString(".000"), handleNeg), ctx.MkExtract(handleNeg, ctx.MkInt(0), (IntExpr)ctx.MkSub(ctx.MkLength(handleNeg), ctx.MkInt(4))), handleNeg);

                //var z = (SeqExpr)ctx.MkConst("z", ctx.StringSort);
                //solver.Assert(ctx.MkEq(z, handleEnd));

                var status = solver.Check();
                if (status == Status.SATISFIABLE)
                {
                    foreach (var item in solver.Model.Consts)
                    {
                        var v = solver.Model.Eval(item.Value);
                        Console.WriteLine($"{item.Key.Name} = {v}");
                    }
                }
                else
                    Console.WriteLine(status);
            }
        }

        [TestMethod]
        public void Z3()
        {
            //z(Number)=-0.5==(:a)
            //ab(Number)=(z)
            //bb(Number)=(ab)
            //db(Number)=-((bb))
            //eb(Number)=6+db
            //goto eb

            using (var ctx = new Context())
            using (var solver = ctx.MkSolver())
            {
                var z = ctx.MkConst("z", ctx.IntSort);
                solver.Assert(ctx.MkOr(ctx.MkEq(z, ctx.MkInt(1000)), ctx.MkEq(z, ctx.MkInt(0000))));

                var ab = ctx.MkConst("ab", ctx.IntSort);
                solver.Assert(ctx.MkEq(ab, z));

                var bb = ctx.MkConst("bb", ctx.IntSort);
                solver.Assert(ctx.MkEq(bb, ab));

                var db = ctx.MkConst("db", ctx.IntSort);
                solver.Assert(ctx.MkEq(bb, ctx.MkMul(ctx.MkInt(-1), (IntExpr)db)));

                var eb = ctx.MkConst("eb", ctx.IntSort);
                solver.Assert(ctx.MkEq(eb, ctx.MkAdd(ctx.MkInt(6000), (IntExpr)db)));

                var @goto = (IntExpr)ctx.MkConst("goto", ctx.IntSort);

                var x = ctx.MkITE(ctx.MkLt((IntExpr)eb, ctx.MkInt(1000)), ctx.MkInt(1000), eb);
                x = ctx.MkITE(ctx.MkGt((IntExpr)x, ctx.MkInt(20000)), ctx.MkInt(20000), eb);
                x = ctx.MkDiv((IntExpr)x, ctx.MkInt(1000));
                solver.Assert(ctx.MkEq(@goto, x));

                while (solver.Check() == Status.SATISFIABLE)
                {
                    var v = (IntNum)solver.Model.Eval(@goto);
                    Console.WriteLine(v);

                    solver.Assert(ctx.MkNot(ctx.MkEq(@goto, v)));
                }

                Console.WriteLine(solver.Check());
            }
        }
    }
}

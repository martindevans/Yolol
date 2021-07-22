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

        [TestMethod]
        public void Superoptimise_Attempt2()
        {
            using var ctx = new Context();
            using var solver = ctx.MkSolver();
            solver.Set("timeout", 100);

            // :o = (i%a * i%b * i%c * i%d * i%e * i%f)>g

            // Setup params we want to know
            var a = (IntExpr)ctx.MkConst("a", ctx.IntSort);
            var b = (IntExpr)ctx.MkConst("b", ctx.IntSort);
            var c = (IntExpr)ctx.MkConst("c", ctx.IntSort);
            var d = (IntExpr)ctx.MkConst("d", ctx.IntSort);
            var e = (IntExpr)ctx.MkConst("e", ctx.IntSort);
            var f = (IntExpr)ctx.MkConst("f", ctx.IntSort);
            var g = (IntExpr)ctx.MkConst("g", ctx.IntSort);

            solver.Assert(a > 0 & a < 20);
            solver.Assert(b > 0 & b < 20);
            solver.Assert(c > 0 & c < 20);
            solver.Assert(d > 0 & d < 20);
            solver.Assert(e > 0 & e < 20);
            solver.Assert(f > 0 & f < 20);
            solver.Assert(g >= 0 & g < 1);

            //solver.Assert(ctx.MkEq(a, ctx.MkInt(2)));
            //solver.Assert(ctx.MkEq(b, ctx.MkInt(3)));
            solver.Assert(ctx.MkEq(c, ctx.MkInt(5)));
            solver.Assert(ctx.MkEq(d, ctx.MkInt(7)));
            solver.Assert(ctx.MkEq(e, ctx.MkInt(11)));
            solver.Assert(ctx.MkEq(f, ctx.MkInt(13)));
            solver.Assert(ctx.MkEq(g, ctx.MkInt(0)));

            // Setup cases
            BoolExpr Equation(int i)
            {
                var ii = ctx.MkInt(i);
                var aa = ctx.MkRem(ii, a);
                var bb = ctx.MkRem(ii, b);
                var cc = ctx.MkRem(ii, c);
                var dd = ctx.MkRem(ii, d);
                var ee = ctx.MkRem(ii, e);
                var ff = ctx.MkRem(ii, f);

                var expr = (aa * bb * cc * dd * ee * ff);
                return expr > g;
            }

            for (var i = 14; i < 201; i++)
                solver.Assert(ctx.MkEq(ctx.MkBool(IsPrime(i)), Equation(i)));

            static bool IsPrime(int number)
            {
                if (number <= 1) return false;
                if (number == 2) return true;
                if (number % 2 == 0) return false;

                var boundary = (int)Math.Floor(Math.Sqrt(number));

                for (int i = 3; i <= boundary; i+=2)
                    if (number % i == 0)
                        return false;

                return true;        
            }

            if (solver.Check() == Status.SATISFIABLE)
            {
                var av = (IntNum)solver.Model.Eval(a);
                var bv = (IntNum)solver.Model.Eval(b);
                var cv = (IntNum)solver.Model.Eval(c);
                var dv = (IntNum)solver.Model.Eval(d);
                var ev = (IntNum)solver.Model.Eval(e);
                var fv = (IntNum)solver.Model.Eval(f);
                var gv = (IntNum)solver.Model.Eval(g);

                Console.WriteLine($":o = (i%{av}+i%{bv}+i%{cv}+i%{dv}+i%{ev}+i%{fv})>{gv}");
            }

            Console.WriteLine(solver.Check());
        }

        enum Op
        {
            And, Or, Xor
        }

        [TestMethod]
        public void Superoptimise_Attempt()
        {
            // 		0 1 2
            // and  0 0 1   o<x
            // or   0 1 1   o==x
            // xor  0 1 0   o>x

            using var ctx = new Context();
            using var solver = ctx.MkSolver();
            solver.Set("timeout", 1000);

            //:o=(c+OFFSET+is_and*P1+is_xor*P2+c*is_xor_or*P3+is_or_and*P4)%P5>P6

            // Setup params we want to know
            var offset = (IntExpr)ctx.MkConst("OFFSET", ctx.IntSort);
            var p1 = (IntExpr)ctx.MkConst("P1", ctx.IntSort);
            var p2 = (IntExpr)ctx.MkConst("P2", ctx.IntSort);
            var p3 = (IntExpr)ctx.MkConst("P3", ctx.IntSort);
            var p4 = (IntExpr)ctx.MkConst("P4", ctx.IntSort);
            var p5 = (IntExpr)ctx.MkConst("P5", ctx.IntSort);
            var p6 = (IntExpr)ctx.MkConst("P6", ctx.IntSort);

            solver.Assert(offset > -10 & offset < 10);
            solver.Assert(p1 > 0 & p1 < 10);
            solver.Assert(p2 > 0 & p2 < 10);
            solver.Assert(p3 > 0 & p3 < 10);
            solver.Assert(p4 > -5 & p4 < 10);
            solver.Assert(p5 > 0 & p5 < 10);
            solver.Assert(p6 > -10 & p6 < 10);

            // Setup cases
            BoolExpr Equation(Op op, int sum)
            {
                var c = (IntExpr)ctx.MkInt(sum);

                var is_or_and = (IntExpr)ctx.MkInt(op != Op.Xor ? 1 : 0);
                var is_and = (IntExpr)ctx.MkInt(op == Op.And ? 1 : 0);
                var is_or_xor = (IntExpr)ctx.MkInt(op != Op.And ? 1 : 0);
                var is_xor = (IntExpr)ctx.MkInt(op == Op.Xor ? 1 : 0);

                var a = (IntExpr)(c + offset + c * is_and * p1 + is_xor * p2 + is_or_xor * p3 + is_or_and * p4);
                var b = ctx.MkMod(a, p5);

                return b > p6;
            }

            solver.Assert(ctx.MkEq(ctx.MkFalse(), Equation(Op.And, 0)));
            solver.Assert(ctx.MkEq(ctx.MkFalse(), Equation(Op.And, 1)));
            solver.Assert(ctx.MkEq(ctx.MkTrue(), Equation(Op.And, 2)));

            solver.Assert(ctx.MkEq(ctx.MkFalse(), Equation(Op.Or, 0)));
            solver.Assert(ctx.MkEq(ctx.MkTrue(), Equation(Op.Or, 1)));
            solver.Assert(ctx.MkEq(ctx.MkTrue(), Equation(Op.Or, 2)));

            solver.Assert(ctx.MkEq(ctx.MkFalse(), Equation(Op.Xor, 0)));
            solver.Assert(ctx.MkEq(ctx.MkTrue(), Equation(Op.Xor, 1)));
            solver.Assert(ctx.MkEq(ctx.MkFalse(), Equation(Op.Xor, 2)));

            if (solver.Check() == Status.SATISFIABLE)
            {
                var offsetv = (IntNum)solver.Model.Eval(offset);
                var p1v = (IntNum)solver.Model.Eval(p1);
                var p2v = (IntNum)solver.Model.Eval(p2);
                var p3v = (IntNum)solver.Model.Eval(p3);
                var p4v = (IntNum)solver.Model.Eval(p4);
                var p5v = (IntNum)solver.Model.Eval(p5);
                var p6v = (IntNum)solver.Model.Eval(p6);

                Console.WriteLine($":o=(:a+:b+{offsetv}+is_and*{p1v}+is_xor*{p2v}+is_x_or*{p3v}+is_or_and*{p4v})%{p5v}>{p6v}");

                //var code = $"o=(:a+:b+{offsetv}+({0}==\"and\")*{p1v}+({0}==\"xor\")*{p2v}+({0}!=\"and\")*{p3v})%{p4v}>{p5v}";

                //foreach (var op in new[] { "and", "or", "xor" })
                //{
                //    var results = Enumerable.Range(0, 3).Select(i => {
                //        var c = string.Format(code, op, i);
                //        var r = TestExecutor.Execute(c);
                //        return r.GetVariable("o").Value.Number;
                //    });

                //    Console.WriteLine(op + string.Join("", Enumerable.Repeat(" ", 5 - op.Length)) + string.Join(" ", results));
                //}

                //solver.Assert(ctx.MkNot(ctx.MkEq(c, cv)));
            }

            Console.WriteLine(solver.Check());
        }
    }
}

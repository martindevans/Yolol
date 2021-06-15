using System;
using Yolol.Execution;
using Yolol.Execution.Extensions;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Expressions.Binary;

namespace Yolol.Analysis.TreeVisitor.Reduction
{
    public class ConstantCompressor
        : BaseTreeVisitor
    {
        protected override BaseExpression Visit(ConstantNumber con)
        {
            if (con.Value.ToString().Length <= 3)
                return base.Visit(con);

            var replacements = new[] {
                SmallestExponents(con.Value),
                BestFraction(con.Value),
            };

            var shortestLength = int.MaxValue;
            BaseExpression? shortest = null;
            foreach (var replacement in replacements)
            {
                if (replacement == null)
                    continue;

                var l = replacement.ToString().Length;
                if (l < shortestLength)
                {
                    shortestLength = l;
                    shortest = replacement;
                }
            }

            if (shortest != null && shortestLength < con.ToString().Length)
                return base.Visit(shortest);

            return base.Visit(con);
        }

        private static BaseExpression? SmallestExponents(Number value)
        {
            var bestLength = int.MaxValue;
            BaseExpression? best = null;

            void Submit(BaseExpression exp)
            {
                if (exp.StaticEvaluate().Number != value)
                    return;

                var length = exp.ToString().Length;
                if (length < bestLength)
                {
                    bestLength = length;
                    best = exp;
                }
            }

            for (var idx = 1; idx < 320; idx++)
            {
                var b = idx / 10m;
                try
                {
                    var log = Math.Log((double)value, (double)b);
                    if (double.IsNaN(log))
                        continue;

                    var exp = new Exponent(new ConstantNumber((Number)b), new ConstantNumber((Number)log));
                    Submit(exp);

                    var integral = new Exponent(new ConstantNumber((Number)b), new ConstantNumber((Number)(int)Math.Round(log)));
                    var integralV = integral.StaticEvaluate().Number;
                    var integralE = value - integralV;
                    Submit(new Add(integral, new ConstantNumber(integralE)));
                }
                catch (OverflowException)
                {
                    // If the calculation overflows then don't submit this case
                }
            }

            return best;
        }

        private static BaseExpression BestFraction(Number number)
        {
            static (double, double) RealToFraction(double value, double accuracy)
            {
                if (accuracy <= 0.0 || accuracy >= 1.0)
                    throw new ArgumentOutOfRangeException(nameof(accuracy), "Must be > 0 and < 1.");

                var sign = Math.Sign(value);

                if (sign == -1)
                {
                    value = Math.Abs(value);
                }

                // Accuracy is the maximum relative error; convert to absolute maxError
                var maxError = sign == 0 ? accuracy : value * accuracy;

                var n = (int) Math.Floor(value);
                value -= n;

                if (value < maxError)
                    return (sign * n, 1);

                if (1 - maxError < value)
                    return (sign * (n + 1), 1);

                // The lower fraction is 0/1
                var lowerN = 0;
                var lowerD = 1;

                // The upper fraction is 1/1
                var upperN = 1;
                var upperD = 1;

                while (true)
                {
                    // The middle fraction is (lower_n + upper_n) / (lower_d + upper_d)
                    var middleN = lowerN + upperN;
                    var middleD = lowerD + upperD;

                    if (middleD * (value + maxError) < middleN)
                    {
                        // real + error < middle : middle is our new upper
                        upperN = middleN;
                        upperD = middleD;
                    }
                    else if (middleN < (value - maxError) * middleD)
                    {
                        // middle < real - error : middle is our new lower
                        lowerN = middleN;
                        lowerD = middleD;
                    }
                    else
                    {
                        // Middle is our best fraction
                        return ((n * middleD + middleN) * sign, middleD);
                    }
                }
            }

            if (number >= Number.One)
                return new ConstantNumber(number);

            var (fn, fd) = RealToFraction((double)number, 0.001f);

            var replacement = new Divide(new ConstantNumber((Number)fn), new ConstantNumber((Number)fd));

            if (replacement.StaticEvaluate().Number == number)
                return replacement;
            else
                return new ConstantNumber(number);
        }
    }
}

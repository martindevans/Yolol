using System.Linq;
using JetBrains.Annotations;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Expressions.Binary;
using Yolol.Grammar.AST.Expressions.Unary;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.Reduction
{
    public class IfAssignmentCompression
        : BaseTreeVisitor
    {
        protected override BaseStatement Visit(If @if)
        {
            if (@if.TrueBranch.Statements.Count > 1)
                return base.Visit(@if);

            if (!(@if.TrueBranch.Statements.Single() is Assignment trueAss))
                return base.Visit(@if);

            var condition = @if.Condition.IsBoolean
                ? new Bracketed(@if.Condition)
                : new Bracketed(new EqualTo(new Bracketed(@if.Condition), new ConstantNumber(1)));

            if (!trueAss.Right.IsConstant)
                return base.Visit(@if);

            var trueRight = StaticEvaluate(trueAss.Right);
            if (trueRight.Type != Execution.Type.Number)
                return base.Visit(@if);

            if (@if.FalseBranch.Statements.Count == 0)
            {
                // Replace:
                //      `if cond then a = v1 end`
                // With
                //      `a += (v1 - a) * cond
                return new CompoundAssignment(trueAss.Left, Grammar.YololBinaryOp.Add, new Multiply(new Bracketed(new Subtract(new ConstantNumber(trueRight.Number), new Variable(trueAss.Left))), condition));
            }
            else
            {
                if (@if.FalseBranch.Statements.Count != 1)
                    return base.Visit(@if);

                if (!(@if.FalseBranch.Statements.Single() is Assignment falseAss))
                    return base.Visit(@if);

                if (trueAss.Left.Name != falseAss.Left.Name)
                    return base.Visit(@if);

                var falseRight = StaticEvaluate(falseAss.Right);
                if (falseRight.Type != Execution.Type.Number)
                    return base.Visit(@if);

                // Replace:
                //      `if cond then a = v1 else a = v2 end`
                // With
                //      `a = v2 + (v1 - v2) * cond

                //b = 3 if a == 0 then b = 5 else b = 10 end

                var diff = new Subtract(new ConstantNumber(trueRight.Number), new ConstantNumber(falseRight.Number));
                var diffEval = StaticEvaluate(diff);
                var finalRight = diffEval.Number == 1 ? (BaseExpression)condition : new Bracketed(new Multiply(new ConstantNumber(diffEval.Number), condition));

                return new Assignment(trueAss.Left, new Add(new ConstantNumber(falseRight.Number), finalRight));
            }
        }
    }
}

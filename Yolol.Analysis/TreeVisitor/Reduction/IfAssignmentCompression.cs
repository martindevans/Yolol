using System;
using System.Linq;
using Yolol.Analysis.Types;
using Yolol.Execution;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Expressions.Binary;
using Yolol.Grammar.AST.Expressions.Unary;
using Yolol.Grammar.AST.Statements;
using Variable = Yolol.Grammar.AST.Expressions.Variable;

namespace Yolol.Analysis.TreeVisitor.Reduction
{
    public class IfAssignmentCompression
        : BaseTreeVisitor
    {
        private readonly ITypeAssignments _types;

        public IfAssignmentCompression(ITypeAssignments types)
        {
            _types = types;
        }

        protected override BaseStatement Visit(If @if)
        {
            // Make sure true branch contains:
            // - A statement
            // - Which is an assignment
            // - To a number typed variable
            // - Variable is not external
            if (@if.TrueBranch.Statements.Count > 1)
                return base.Visit(@if);
            if (!(@if.TrueBranch.Statements.SingleOrDefault() is Assignment trueAss))
                return base.Visit(@if);
            if (_types.TypeOf(trueAss.Left) != Execution.Type.Number)
                return base.Visit(@if);
            if (trueAss.Left.IsExternal)
                return base.Visit(@if);

            // Make sure the condition is something that returns `0` or `1`
            // If the `condition` is not in this form already then replace it with `condition != 0`
            var condition = @if.Condition.IsBoolean
                ? new Bracketed(@if.Condition)
                : new Bracketed(new NotEqualTo(new Bracketed(@if.Condition), new ConstantNumber((Number)0)));

            if (@if.FalseBranch.Statements.Count == 0)
            {
                // Replace:
                //      `if cond then a = v1 end`
                // With
                //      `a += (v1 - a) * cond
                return new CompoundAssignment(trueAss.Left, Grammar.YololBinaryOp.Add, new Multiply(new Bracketed(new Subtract(new Bracketed(trueAss.Right), new Variable(trueAss.Left))), condition));
            }
            else
            {
                // todo: implement false branch
                throw new NotImplementedException();

                //if (@if.FalseBranch.Statements.Count != 1)
                //    return base.Visit(@if);

                //if (!(@if.FalseBranch.Statements.Single() is Assignment falseAss))
                //    return base.Visit(@if);

                //if (trueAss.Left.Name != falseAss.Left.Name)
                //    return base.Visit(@if);

                //var falseRight = falseAss.Right.StaticEvaluate();
                //if (falseRight.Type != Execution.Type.Number)
                //    return base.Visit(@if);

                //// Replace:
                ////      `if cond then a = v1 else a = v2 end`
                //// With
                ////      `a = v2 + (v1 - v2) * cond

                ////b = 3 if a == 0 then b = 5 else b = 10 end

                //var diff = new Subtract(new ConstantNumber(trueRight.Number), new ConstantNumber(falseRight.Number));
                //var diffEval = diff.StaticEvaluate();
                //var finalRight = diffEval.Number == 1 ? (BaseExpression)condition : new Bracketed(new Multiply(new ConstantNumber(diffEval.Number), condition));

                //return new Assignment(trueAss.Left, new Add(new ConstantNumber(falseRight.Number), finalRight));
            }
        }
    }
}

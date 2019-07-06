using Yolol.Execution;
using Yolol.Execution.Extensions;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Expressions.Unary;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.TreeVisitor.Reduction
{
    public class ConstantFoldingVisitor
        : BaseTreeVisitor
    {
        protected override BaseExpression Visit(BaseExpression expression)
        {
            if (expression.IsConstant)
            {
                var v = expression.Evaluate(new MachineState(new NullDeviceNetwork(), new DefaultIntrinsics()));

                //// Do not substitute the constant value if it is a longer string than the expression
                //if (v.ToString().Length > expression.ToString().Length)
                //    return base.Visit(expression);

                if (v.Type == Type.Number)
                    return base.Visit(new ConstantNumber(v.Number));
                else
                    return base.Visit(new ConstantString(v.String));
            }

            return base.Visit(expression);
        }

        protected override BaseStatement Visit(If @if)
        {
            if (!@if.Condition.IsConstant)
                return base.Visit(@if);

            var cond = @if.Condition.StaticEvaluate();

            // Condition must be a number, if we've constant folded to a string emit the original if statement unchanged
            if (cond.Type != Type.Number)
                return base.Visit(@if);

            if (cond.Number != 0)
                return Visit(@if.TrueBranch);
            else
                return Visit(@if.FalseBranch);
        }
    }
}

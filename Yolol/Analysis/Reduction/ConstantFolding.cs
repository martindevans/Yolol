using JetBrains.Annotations;
using Yolol.Execution;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Expressions.Unary;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.Reduction
{
    public class ConstantFoldingVisitor
        : BaseTreeVisitor
    {
        public override BaseExpression Visit(BaseExpression expression)
        {
            if (expression.IsConstant)
            {
                var v = expression.Evaluate(new MachineState(new NullDeviceNetwork(), new DefaultIntrinsics()));
                if (v.Type == Type.Number)
                    return new ConstantNumber(v.Number);
                else
                    return new ConstantString(v.String);
            }

            return expression;
        }

        protected override BaseStatement Visit(If @if)
        {
            if (!@if.Condition.IsConstant)
                return @if;

            var cond = Evaluate(@if.Condition);

            // Condition must be a number, if we've constant folded to a string emit the original if statement unchanged
            if (cond.Type != Type.Number)
                return @if;

            if (cond.Number != 0)
                return @if.TrueBranch;
            else
                return @if.FalseBranch;
        }

        private static Value Evaluate([NotNull] BaseExpression expression)
        {
            return expression.Evaluate(new MachineState(new NullDeviceNetwork(), new DefaultIntrinsics()));
        }
    }
}

using Yolol.Execution;
using Yolol.Execution.Extensions;
using Yolol.Grammar.AST.Expressions;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.TreeVisitor.Reduction
{
    public class ConstantFoldingVisitor
        : BaseTreeVisitor
    {
        private readonly bool _allowLonger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="allowLonger">Set to true to allow constant folding even when it results in longer code</param>
        public ConstantFoldingVisitor(bool allowLonger = false)
        {
            _allowLonger = allowLonger;
        }

        public override BaseExpression Visit(BaseExpression expression)
        {
            if (expression.IsConstant)
            {
                var v = expression.TryStaticEvaluate();
                if (v.HasValue)
                {
                    // Do not substitute the constant value if it is a longer string than the expression
                    if (!_allowLonger && v.ToString()!.Length > expression.ToString().Length)
                        return base.Visit(expression);

                    if (v.Value.Type == Type.Number)
                        return base.Visit(new ConstantNumber(v.Value.Number));
                    else
                        return base.Visit(new ConstantString(v.Value.String));
                }
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

            if (cond.Number != Number.Zero)
                return Visit(@if.TrueBranch);
            else
                return Visit(@if.FalseBranch);
        }
    }
}

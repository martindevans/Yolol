using Yolol.Execution.Extensions;
using Yolol.Grammar.AST.Expressions.Unary;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.TreeVisitor.Reduction
{
    public class CompoundCompressor
        : BaseTreeVisitor
    {
        protected override BaseStatement Visit(CompoundAssignment compAss)
        {
            if (compAss.Op != Grammar.YololBinaryOp.Add && compAss.Op != Grammar.YololBinaryOp.Subtract)
                return base.Visit(compAss);

            if (!compAss.Expression.IsConstant)
                return base.Visit(compAss);

            var value = compAss.Expression.StaticEvaluate();
            if (value.Type != Execution.Type.Number || value.Number != 1)
                return base.Visit(compAss);

            if (compAss.Op == Grammar.YololBinaryOp.Add)
                return new ExpressionWrapper(new PostIncrement(compAss.Left));
            else
                return new ExpressionWrapper(new PostDecrement(compAss.Left));
        }
    }
}

using Yolol.Analysis.ControlFlowGraph.AST;
using Yolol.Analysis.Types;
using Yolol.Execution.Extensions;
using Yolol.Grammar.AST.Statements;

using Type = Yolol.Execution.Type;

namespace Yolol.Analysis.TreeVisitor.Reduction
{
    public class CompoundCompressor
        : BaseTreeVisitor
    {
        private readonly ITypeAssignments _types;

        public CompoundCompressor(ITypeAssignments types)
        {
            _types = types;
        }

        protected override BaseStatement Visit(CompoundAssignment compAss)
        {
            if (compAss.Op != Grammar.YololBinaryOp.Add && compAss.Op != Grammar.YololBinaryOp.Subtract)
                return base.Visit(compAss);

            if (!compAss.Expression.IsConstant)
                return base.Visit(compAss);

            // if right side cannot be evaluated then we can't compress it
            var right = compAss.Expression.TryStaticEvaluate();
            if (!right.HasValue)
                return base.Visit(compAss);

            var type = _types.TypeOf(compAss.Left);

            switch (type)
            {
                default:
                case null:
                case Type.Error:
                case Type.Unassigned:
                    return base.Visit(compAss);

                case Type.Number: {

                    if (!right.Value.Equals(1))
                        return base.Visit(compAss);

                    if (compAss.Op == Grammar.YololBinaryOp.Add)
                        return new Assignment(compAss.Left, new Increment(compAss.Left));
                    else
                        return new Assignment(compAss.Left, new Decrement(compAss.Left));
                }

                case Type.String: {

                    if (!right.Value.Equals(" ") || compAss.Op != Grammar.YololBinaryOp.Add)
                        return base.Visit(compAss);

                    return new Assignment(compAss.Left, new Increment(compAss.Left));
                }
            }
        }
    }
}

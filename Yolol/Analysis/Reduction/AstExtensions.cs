using JetBrains.Annotations;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.Reduction
{
    public static class AstExtensions
    {
        [NotNull] public static Line FoldConstants([NotNull] this Line line)
        {
            return new ConstantFoldingVisitor().Visit(line);
        }

        [NotNull] public static Line SimplifyVariableNames([NotNull] this Line line)
        {
            return new VariableSimplificationVisitor().Visit(line);
        }
    }
}

using JetBrains.Annotations;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.Reduction
{
    public static class AstExtensions
    {
        [NotNull] public static Program FoldConstants([NotNull] this Program prog)
        {
            return new ConstantFoldingVisitor().Visit(prog);
        }

        [NotNull] public static Program SimplifyVariableNames([NotNull] this Program prog)
        {
            return new VariableSimplificationVisitor().Visit(prog);
        }

        //[NotNull] public static Program HoistConstants([NotNull] this Program prog)
        //{
        //    return new ConstantHoisting().Visit(prog);
        //}
    }
}

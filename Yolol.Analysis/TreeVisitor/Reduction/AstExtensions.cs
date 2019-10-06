using JetBrains.Annotations;
using Yolol.Grammar.AST;
using Yolol.Grammar.AST.Statements;

namespace Yolol.Analysis.TreeVisitor.Reduction
{
    public static class AstExtensions
    {
        [NotNull] public static Program SimplifyVariableNames([NotNull] this Program prog)
        {
            return new VariableSimplificationVisitor().Visit(prog);
        }

        [NotNull] public static Program CompressCompoundIncrement([NotNull] this Program prog)
        {
            return new CompoundCompressor().Visit(prog);
        }

        [NotNull] public static Program DeadPostGotoElimination([NotNull] this Program prog)
        {
            return new DeadStatementAfterGotoElimination().Visit(prog);
        }

        [NotNull] public static Program TrailingGotoNextLineElimination([NotNull] this Program prog)
        {
            return new EndOfLineGotoElimination().Visit(prog);
        }

        [NotNull] public static Program TrailingConditionalGotoAnyLineCompression([NotNull] this Program prog)
        {
            return new IfThenGotoCompressor().Visit(prog);
        }

        [NotNull] public static Program ConditionalGotoCompression([NotNull] this Program prog, [NotNull] INameGenerator names)
        {
            return new IfThenElseGotoCompressor(names).Visit(prog);
        }

        [NotNull] public static Program CompressConditionalAssignment([NotNull] this Program prog)
        {
            return new IfAssignmentCompression().Visit(prog);
        }

        


        [NotNull] public static Program FoldConstants([NotNull] this Program prog)
        {
            return new ConstantFoldingVisitor().Visit(prog);
        }

        [NotNull] public static Program HoistConstants([NotNull] this Program prog)
        {
            return new RepeatConstantHoisting().Visit(prog);
        }

        [NotNull] public static Program CompressConstants([NotNull] this Program prog)
        {
            return new ConstantCompressor().Visit(prog);
        }




        [NotNull] public static Program StripTypes([NotNull] this Program prog)
        {
            return new StripTypes().Visit(prog);
        }
    }
}

using JetBrains.Annotations;
using Yolol.Grammar.AST;

namespace Yolol.Analysis.TreeVisitor.Reduction
{
    public static class AstExtensions
    {
        /// <summary>
        /// Remove brackets where they are not needed in some trivial cases
        /// </summary>
        /// <param name="prog"></param>
        /// <returns></returns>
        [NotNull] public static Program SimpleBracketElimination([NotNull] this Program prog)
        {
            return new SimpleBracketElimination().Visit(prog);
        }

        /// <summary>
        /// Replace variables with simpler alternative names
        /// </summary>
        /// <param name="prog"></param>
        /// <returns></returns>
        [NotNull] public static Program SimplifyVariableNames([NotNull] this Program prog)
        {
            return new VariableSimplificationVisitor().Visit(prog);
        }

        /// <summary>
        /// Remove all the code after an unconditional goto
        /// </summary>
        /// <param name="prog"></param>
        /// <returns></returns>
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

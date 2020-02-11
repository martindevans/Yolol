using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Yolol.Analysis.TreeVisitor.Inspection;
using Yolol.Analysis.TreeVisitor.Modification;
using Yolol.Grammar.AST;

namespace Yolol.Analysis.TreeVisitor.Reduction
{
    public static class AstExtensions
    {
        /// <summary>
        /// Remove variables assigned to themselves
        /// </summary>
        /// <param name="prog"></param>
        /// <returns></returns>
        [NotNull]
        public static Program SelfAssignmentElimination([NotNull] this Program prog)
        {
            return new SelfAssignmentElimination().Visit(prog);
        }

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


        [NotNull]  public static Program FoldSingleUseVariables([NotNull] this Program prog)
        {
            // Find all variables associated with a count of how many times they are read
            var readsInProgram = new FindReadVariables();
            readsInProgram.Visit(prog);

            // Find all variables associated with a count of how many times they are assigned
            var writesInProgram = new FindAssignedVariables();
            writesInProgram.Visit(prog);

            var lines = new List<Line>();

            foreach (var item in prog.Lines)
            {
                lines.Add(item.Fixpoint(20, line => {

                    // Find all reads in this line
                    var readsInLine = new FindReadVariables();
                    readsInLine.Visit(line);

                    // Find all writes in this line
                    var writesInLine = new FindAssignedVariables();
                    writesInLine.Visit(line);

                    // Find variables that can be folded away
                    var toReplace = (from v in readsInLine.Counts
                                     where !v.Key.IsExternal
                                     where v.Value == 1                         // Filter to things only read once in this line
                                     where readsInProgram.Counts[v.Key] == 1    // Filter to things only read once in the entire program
                                     where writesInProgram.Counts[v.Key] == 1   // Filter to things on written once in the entire program
                                     where writesInLine.Counts[v.Key] == 1      // Filter to things written once in this line
                                     select v.Key).FirstOrDefault();

                    // If nothing was found to fold we can break out of the loop
                    if (toReplace == null)
                        return line;

                    // Find the value that was assigned to this
                    var expr = writesInLine.Expressions[toReplace].Single();

                    // Substitute it into the line
                    var result = new SubstituteVariable(toReplace, expr).Visit(line);
                    return result;
                }));
            }

            return new Program(lines);
        }


        [NotNull] public static Program StripTypes([NotNull] this Program prog)
        {
            return new StripTypes().Visit(prog);
        }
    }
}

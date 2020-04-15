using System.Collections.Generic;
using System.Linq;
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
        public static Program SelfAssignmentElimination(this Program prog)
        {
            return new SelfAssignmentElimination().Visit(prog);
        }

        /// <summary>
        /// Remove brackets where they are not needed in some trivial cases
        /// </summary>
        /// <param name="prog"></param>
        /// <returns></returns>
        public static Program SimpleBracketElimination(this Program prog)
        {
            return new SimpleBracketElimination().Visit(prog);
        }

        /// <summary>
        /// Replace variables with simpler alternative names
        /// </summary>
        /// <param name="prog"></param>
        /// <returns></returns>
        public static Program SimplifyVariableNames(this Program prog)
        {
            return new VariableSimplificationVisitor().Visit(prog);
        }

        /// <summary>
        /// Remove all the code after an unconditional goto
        /// </summary>
        /// <param name="prog"></param>
        /// <returns></returns>
        public static Program DeadPostGotoElimination(this Program prog)
        {
            return new DeadStatementAfterGotoElimination().Visit(prog);
        }

        public static Program TrailingGotoNextLineElimination(this Program prog)
        {
            return new EndOfLineGotoElimination().Visit(prog);
        }

        public static Program TrailingConditionalGotoAnyLineCompression(this Program prog)
        {
            return new IfThenGotoCompressor().Visit(prog);
        }

        public static Program ConditionalGotoCompression(this Program prog, INameGenerator names)
        {
            return new IfThenElseGotoCompressor(names).Visit(prog);
        }


        public static Program FoldConstants(this Program prog)
        {
            return new ConstantFoldingVisitor().Visit(prog);
        }

        public static Program HoistConstants(this Program prog)
        {
            return new RepeatConstantHoisting().Visit(prog);
        }

        public static Program CompressConstants(this Program prog)
        {
            return new ConstantCompressor().Visit(prog);
        }


         public static Program FoldSingleUseVariables(this Program prog)
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


        public static Program StripTypes(this Program prog)
        {
            return new StripTypes().Visit(prog);
        }
    }
}

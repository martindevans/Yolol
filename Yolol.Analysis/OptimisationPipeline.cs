using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Yolol.Analysis.ControlFlowGraph;
using Yolol.Analysis.ControlFlowGraph.Extensions;
using Yolol.Analysis.Fuzzer;
using Yolol.Analysis.TreeVisitor.Reduction;
using Yolol.Analysis.Types;
using Yolol.Grammar;
using Yolol.Grammar.AST;
using Type = Yolol.Execution.Type;

namespace Yolol.Analysis
{
    public class OptimisationPipeline
    {
        private readonly (VariableName, Type)[] _typeHints;
        private readonly int _itersLimit;
        private readonly bool _keepTypes;

        /// <summary>
        /// How many unique runs of the program will the fuzzer run before and after optimisation to verify correctness
        /// </summary>
        public int FuzzSafetyRuns { get; set; } = 250;

        /// <summary>
        /// How many lines will each fuzzer run execute (max)
        /// </summary>
        public int FuzzSafetyIterations { get; set; } = 50;

        /// <summary>
        /// Disable all AST bsed optimisation passes
        /// </summary>
        public bool DisableAstPasses { get; set; }

        /// <summary>
        /// Disable all CFG based optimisation passes
        /// </summary>
        public bool DisableCfgPasses { get; set; }


        public OptimisationPipeline([NotNull] (VariableName, Type)[] typeHints)
        {
            _typeHints = typeHints;
            _itersLimit = int.MaxValue;
        }

        public OptimisationPipeline(int itersLimit, bool keepTypes, (VariableName, Type)[] typeHints)
        {
            _typeHints = typeHints;
            _itersLimit = itersLimit;
            _keepTypes = keepTypes;
        }

        [NotNull] public async Task<Program> Apply([NotNull] Program input)
        {
            var fuzz = new QuickFuzz(_typeHints);
            var startFuzz = fuzz.Fuzz(input, FuzzSafetyRuns, FuzzSafetyIterations);

            var count = 0;
            var result = input.Fixpoint(_itersLimit, p => {

                // Remove types  (builder cannot take a typed program)
                p = p.StripTypes();

                // Convert input into control flow graph and optimise
                if (!DisableCfgPasses)
                    p = Optimise(new Builder(p).Build()).ToYolol();

                // Apply simple AST optimisations
                if (!DisableAstPasses)
                    for (var i = 0; i < 2; i++)
                        p = Optimise(p);

                count++;
                Console.WriteLine($"## Pass {count}");
                Console.WriteLine(p);
                Console.WriteLine();

                return p;
            });

            // Strip types if necessary
            if (!_keepTypes)
                result = result.StripTypes();

            // Check that the fuzz test results are the same before and after optimisation
            var end = await fuzz.Fuzz(result);
            if (!CheckFuzz(await startFuzz, end))
                throw new InvalidOperationException("Fuzz test failed - this program encountered an optimisation bug");

            return result;
        }

        private static bool CheckFuzz([NotNull] IFuzzResult startFuzz, [NotNull] IFuzzResult endFuzz)
        {
            if (startFuzz.Count != endFuzz.Count)
                return false;

            for (var i = 0; i < startFuzz.Count; i++)
            {
                var a = startFuzz[i];
                var b = endFuzz[i];

                if (!a.Equals(b))
                {
                    Console.WriteLine("# Fuzz Fail!");
                    Console.WriteLine($"Expected: {a}");
                    Console.WriteLine($"Actual: {b}");
                    Console.WriteLine();
                    return false;
                }
            }

            return true;
        }

        [NotNull] private static Program Optimise(Program program)
        {
            // Replace thing with unnecessary brackets like `(a)` with `a`
            program = program.SimpleBracketElimination();

            // Calculate the value of any constant subexpressions and substitute them into place
            program = program.FoldConstants();

            // Find any constants which are repeated multiple times in the program and store them into a variable on line 1
            program = program.HoistConstants();

            // Find any `goto` statements at the end of a line which goto the next line and remove them
            program = program.TrailingGotoNextLineElimination();

            // Replace:
            //      `if A then goto B end <fallthrough to next line>`
            // With:
            //      `goto B + (Next_Line * (A != 0))`
            program = program.TrailingConditionalGotoAnyLineCompression();

            // Replace:
            //     `if A then goto B else goto C end`
            // With:
            //     goto A*(B-C)+C
            // Or:
            //     x=C goto (A)*((B)-x)+x
            program = program.ConditionalGotoCompression(new RandomNameGenerator(1));

            // Replace constant values with smaller equivalents e.g. replace `a=4294967296` with `a=2^32`
            program = program.CompressConstants();

            // Replace non-external variables with smaller alternatives
            program = program.SimplifyVariableNames();

            return program;
        }

        [NotNull] private IControlFlowGraph Optimise(IControlFlowGraph cf)
        {
            ITypeAssignments types;

            {
                // Convert CFG into SSA form (i.e. each variable is only assigned once)
                cf = cf.StaticSingleAssignment(out var ssa);

                // Infer types for variables
                cf = cf.FlowTypingAssignment(ssa, out types, _typeHints);

                // Replace inc/dec with simpler alternatives for numbers
                cf = cf.SimplifyModificationExpressions(types);

                // Fold away useless mathematics involving constants (e.g. replace `a=x*1` => `a=x` (if x is a number type)
                // This can emit `Error` expressions, which always evaluate to an error
                cf = cf.VisitBlocks(t => new OpNumByConstNumCompressor(t), types);

                // Replace any expressions involving `Error` subexpressions with `Error()` statements
                cf = cf.VisitBlocks(() => new ErrorCompressor());

                // Fold constant expression (e.g. replace `a=2+2` with `a=4`)
                cf = cf.FoldConstants(ssa);

                // Reapply type finding just before we do edge trimming (it's very important we have as many types as possible here and some previous ops may have invalidated them)
                cf = cf.FlowTypingAssignment(ssa, out types, _typeHints);

                // Remove edges in the CFG which cannot happen based on type info (e.g. remove `Error` edges if an error cannot happen, or `Continue` edges if an error is guaranteed to happen)
                cf = cf.TypeDrivenEdgeTrimming(types);

                //Merge together blocks which do not need to be separate any more(e.g.there used to be an error which has now been proven impossible)
                cf = cf.MergeAdjacentBasicBlocks();

                // Remove nodes of the CFG which can never be reached
                cf = cf.RemoveUnreachableBlocks();

                // Remove node of the CFG which contain no statements
                cf = cf.RemoveEmptyBlocks();

                // Replace errors with normal control flow where possible (if a node is guaranteed to error, remove the error and just continue as normal to the next line)
                cf = cf.NormalizeErrors();

                // Remove single static assignment, it is probably too broad after trimming the graph
                cf = cf.RemoveStaticSingleAssignment(ssa);
            }

            {
                // Convert CFG into SSA form (i.e. each variable is only assigned once)
                cf = cf.StaticSingleAssignment(out var ssa);

                // Reapply type finding just before we do edge trimming (it's very important we have as many types as possible here and some previous ops may have invalidated them)
                cf = cf.FlowTypingAssignment(ssa, out types, _typeHints);

                // Fold away unnecessary copies (e.g. replace `b = a c = b` with `b = a c = a`). This leaves useless (unread) variables.
                cf = cf.FoldUnnecessaryCopies(ssa);

                // Replace reads from unassigned variables with `0`
                cf = cf.ReplaceUnassignedReads();

                // Fold constant expression (e.g. replace `a=2+2` with `a=4`)
                cf = cf.FoldConstants(ssa);

                // Find any compound add/sub operations (e.g. `a+=1`) which can be replaced with inc/dec (e.g. `a = inc(a)`)
                cf = cf.VisitBlocks(() => new CompoundCompressor(types));

                // Replace conditional assignments to number fields e.g. `if c then a = b end` with arithmetic `a += (b - a) * c`
                cf = cf.VisitBlocks(() => new IfAssignmentCompression(types));

                // Find all reads in the entire program and then remove any assignments to variables which are never read
                cf = cf.VisitBlocks(u => new RemoveUnreadAssignments(u, ssa), c => c.FindUnreadAssignments());

                // Remove SSA so that it does not interfere with the next iteration
                cf = cf.RemoveStaticSingleAssignment(ssa);
            }

            //cf = cf.RemoveUnreachableBlocks();
            //Console.WriteLine(cf.ToDot());

            return cf;
        }
    }
}
